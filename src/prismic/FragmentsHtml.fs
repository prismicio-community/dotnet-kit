namespace prismic
open FSharp.Data
open FSharp.Data.JsonExtensions
open System
open Fragments
open FragmentsGetters
open ShortcutsAndUtils

module FragmentsHtml =

    type GroupTags = GroupTags of (string option * Block list)
    let imageViewAsHtml ((url, width, height, alt):ImageView) = 
                    String.Format("""<img alt="{0}" src="{1}" width="{2}" height="{3}" />""", (defaultArg alt ""), url, width, height) 

    let rec asHtml (linkResolver:DocumentLink -> string) = function
                    | Link l -> match l with
                                | WebLink (url, _) -> String.Format("""<a href="{0}">{0}</a>""", url)
                                | MediaLink (url, _, _, filename) -> String.Format("""<a href="{0}">{1}</a>""", url, filename)
                                | DocumentLink (l) -> String.Format("""<a href="{0}">{1}</a>""", linkResolver(l), l.slug)
                    | Text t -> String.Format("""<span class="text">{0}</span>""", t)
                    | Date d -> String.Format("""<time>{0}</time>""", (d.ToString("yyyy-MM-dd"))) // Check date time format
                    | Number n -> String.Format("""<span class="number">{0}</span>""", n)
                    | Color c -> String.Format("""<span class="color">{0}</span>""", c)
                    | Embed (typ, provider, url, width, height, html, oembedJson) -> 
                                html 
                                    |> Option.fold (fun _ h -> String.Format("""<div data-oembed="{0}" data-oembed-type="{1}" data-oembed-provider="{2}">{3}</div>""", 
                                                                                    url, typ.ToLowerInvariant(), provider.ToLowerInvariant(), h)
                                                   ) String.Empty
                    | Image (i, m) -> imageViewAsHtml i
                    | Group g ->    let getHtml field fragmentMap = 
                                        get field fragmentMap 
                                            |> Option.bind (fun f -> 
                                                Some(asHtml linkResolver f))
                                    let groupDocsHtml groupDoc = 
                                        let resolve field = 
                                            getHtml field groupDoc <?- ""
                                        groupDoc 
                                            |> Map.toSeq 
                                            |> Seq.map (fun (k, _) -> String.Format("""<section data-field="{0}">{1}</section>""", k, resolve k))
                                    g |> Seq.collect (fun gd -> groupDocsHtml gd) |> String.concat "\n" 
                    | StructuredText t 
                        -> 
                            let embedAsHtml ((typ, provider, url, _, _, html, _):Embed) = 
                                html |> Option.fold (fun s h -> 
                                            String.Format("""<div data-oembed="{0}" data-oembed-type="{1}" data-oembed-provider="{2}">{3}</div>""",
                                                url, typ.ToLowerInvariant(), provider.ToLowerInvariant(), html)) String.Empty
                            let textspanAsHtml (text:string) (spans:Span seq) = 
                                let escape (str:string) = str.Replace("<", "&lt;").Replace("\n", "<br>")
                                let writeTag opening = function
                                                        | Span.Em(_, _) -> if opening then "<em>" else "</em>"
                                                        | Span.Strong(_, _) -> if opening then "<strong>" else "</strong>"
                                                        | Span.Hyperlink(_, _, Link.DocumentLink(l)) 
                                                            -> if opening then String.Format("""<a href="{0}">""", linkResolver l) else "</a>"
                                                        | Span.Hyperlink(_, _, Link.MediaLink(url, _, _, _)) 
                                                            -> if opening then String.Format("""<a href="{0}">""", url) else "</a>"
                                                        | Span.Hyperlink(_, _, Link.WebLink(url, _)) 
                                                            -> if opening then String.Format("""<a href="{0}">""", url) else "</a>"
                                                        //| _ -> if opening then "<span>" else "</span>"
                                let writeHtml endingsToApply startingsToApply = 
                                    let e = endingsToApply 
                                            |> Seq.map (fun e -> writeTag false e) 
                                            |> String.concat "\n"
                                    let s = startingsToApply
                                            |> Seq.map (fun e -> writeTag true e) 
                                            |> String.concat "\n"
                                    String.Format("{0}{1}", e, s)
                                let spanStart = function Span.Em(start, _) | Span.Strong(start, _) | Span.Hyperlink(start, _, _) -> start
                                let spanEnd = function Span.Em(_, end') | Span.Strong(_, end') | Span.Hyperlink(_, end', _) -> end'
                                let rec step in' (startings:Span list) (endings:Span list) (html:hlist<string>) = 
                                    let (<-->) (a:int option) b = [a ; b] |> List.choose id |> function [] -> None | x -> Some(List.min x)
                                    let nextOp = 
                                        startings 
                                            |> seqheadoption 
                                            |> Option.map spanStart
                                        <--> (endings
                                                |> seqheadoption
                                                |> Option.map spanEnd)
                                    match in' with
                                        (_, pos) :: tail when not (nextOp |> Option.exists (fun op -> op = pos))
                                            ->  let (done', todo) = in' |> span (fun (_,i) -> not (nextOp |> Option.exists (fun op -> op = i)))
                                                let consHtml = html =@ escape(System.String(done' |> List.map(fun (c, i) -> c) |> List.toArray))
                                                step todo startings endings consHtml
                                        | (current, pos) :: tail -> 
                                                let (endingsToApply, othersEnding) = endings |> span (fun s -> spanEnd s = pos)
                                                let (startingsToApply, othersStarting) = startings |> span (fun s -> spanStart s = pos)
                                                let applied = String.Format("{0}{1}", writeHtml endingsToApply startingsToApply, escape (current.ToString()))
                                                let moreEndings = startingsToApply |> List.rev
                                                let newEndings = List.append moreEndings othersEnding
                                                let consHtml = html =@ applied
                                                step tail othersStarting newEndings consHtml
                                        | _ -> String.Format("{0}{1}", html |> toList |> String.Concat, writeHtml endings List.Empty)
                                let inText = text.ToCharArray() |> Array.mapi (fun i t -> (t, i)) |> Array.toList
                                step inText (spans |> Seq.sortBy spanStart |> Seq.toList) List.Empty (empty)
                            let blockAsHtml = function
                                    | Block.Text(Text.Heading(text, spans, level)) -> 
                                        String.Format("""<h{0}>{1}</h{0}>""", level, textspanAsHtml text spans)
                                    | Block.Text(Text.Paragraph(text, spans)) ->
                                        String.Format("""<p>{0}</p>""", textspanAsHtml text spans)
                                    | Block.Text(Text.Preformatted(text, spans)) ->
                                        String.Format("""<pre>{0}</pre>""", textspanAsHtml text spans)
                                    | Block.Text(Text.ListItem(text, spans, _)) ->
                                        String.Format("""<li>{0}</li>""", textspanAsHtml text spans)
                                    | Block.Image(view) ->
                                        String.Format("""<p>{0}</p>""", imageViewAsHtml view)
                                    | Block.Embed(embed) -> embedAsHtml embed
                            
                            t   |> Seq.toList |> List.fold (fun s b -> 
                                    match (s, b) with
                                        | (GroupTags(Some("ul"), _) :: _ & GroupTags(ul, blocks) :: tail & group :: _,
                                            Block.Text(Text.ListItem(_, _, false))&block) ->  GroupTags(ul, blocks @ [block]) :: tail
                                        | (GroupTags(Some("ol"), _) :: _ & GroupTags(ul, blocks) :: tail & group :: _, 
                                            Block.Text(Text.ListItem(_, _, true))&block) -> GroupTags(ul, blocks @ [block]) :: tail
                                        | (groups, 
                                            Block.Text(Text.ListItem(_, _, false))&block) -> GroupTags(Some("ul"), [block]) :: groups
                                        | (groups, 
                                            Block.Text(Text.ListItem(_, _, true))&block) -> GroupTags(Some("ol"), [block]) :: groups
                                        | (groups, block) -> GroupTags(None, [block]) :: groups) List.Empty 
                                    |> List.rev
                                |> List.collect (function GroupTags(Some(tag), blocks) 
                                                                -> [String.Format("<{0}>", tag)]
                                                                              @ (blocks |> List.map (fun b -> blockAsHtml b))
                                                                              @ [String.Format("</{0}>", tag)]  // @ to improve
                                                        | GroupTags(None, blocks) 
                                                                ->  (blocks |> List.map (fun b -> blockAsHtml b))) 
                                |> String.concat "\n"
                            

    let getHtml linkResolver field fragmentMap = 
        get field fragmentMap 
            |> Option.bind (fun f -> Some(asHtml linkResolver f))

