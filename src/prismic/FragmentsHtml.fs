namespace prismic
open FSharp.Data
open FSharp.Data.JsonExtensions
open System
open Fragments
open FragmentsGetters
open Shortcuts

module FragmentsHtml =

    type GroupTags = GroupTags of (string option * Block list)

    type OpenSpan = {
        span: Span;
        spanContent: string;
    }

    let imageViewAsHtml (linkResolver:DocumentLink -> string) (view:ImageView) =
        let imgTag = String.Format("""<img alt="{0}" src="{1}" width="{2}" height="{3}" />""", (defaultArg (view.alt) ""), view.url, view.width, view.height)
        match view.linkTo with
            | Some (DocumentLink(l)) -> String.Format("""<a href="{0}">{1}</a>""", linkResolver(l), imgTag)
            | Some (WebLink(l)) -> String.Format("""<a href="{0}">{1}</a>""", l.url, imgTag)
            | Some (MediaLink(l)) -> String.Format("""<a href="{0}">{1}</a>""", l.url, imgTag)
            | Some (ImageLink(l)) -> String.Format("""<a href="{0}">{1}</a>""", l.url, imgTag)
            | _ -> imgTag

    let rec asHtml (linkResolver:DocumentLink -> string) (htmlSerializer:Fragments.Element -> string option) = function
                    | Link l -> match l with
                                | WebLink (l) -> String.Format("""<a href="{0}">{0}</a>""", l.url)
                                | MediaLink (l) -> String.Format("""<a href="{0}">{1}</a>""", l.url, l.filename)
                                | ImageLink (l) -> String.Format("""<a href="{0}">{1}</a>""", l.url, l.name)
                                | DocumentLink (l) -> String.Format("""<a href="{0}">{1}</a>""", linkResolver(l), l.slug)
                    | Text t -> String.Format("""<span class="text">{0}</span>""", htmlEncode t)
                    | Date d -> String.Format("""<time>{0}</time>""", (d.ToUniversalTime().ToString("yyyy-MM-dd"))) // Check date time format
                    | Timestamp d -> String.Format("""<time>{0}</time>""", (d.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss")))
                    | Number n -> String.Format("""<span class="number">{0}</span>""", n)
                    | Color c -> String.Format("""<span class="color">{0}</span>""", c)
                    | GeoPoint g -> String.Format("""<div class="geopoint"><span class="latitude">{0}</span><span class="longitude">{1}</span></div>""", g.latitude, g.longitude)
                    | Embed (e) ->
                                e.html
                                    |> Option.fold (fun _ h -> String.Format("""<div data-oembed="{0}" data-oembed-type="{1}" data-oembed-provider="{2}">{3}</div>""",
                                                                                    e.url, e.typ.ToLowerInvariant(), e.provider.ToLowerInvariant(), h)
                                                   ) String.Empty
                    | Image (i, m) -> imageViewAsHtml linkResolver i
                    | Group g ->    let getHtml field fragmentMap =
                                        get field fragmentMap
                                            |> Option.bind (fun f ->
                                                Some(asHtml linkResolver htmlSerializer f))
                                    let groupDocsHtml groupDoc =
                                        let resolve field =
                                            getHtml field groupDoc <?- ""
                                        groupDoc
                                            |> TupleList.mapKeys (fun k -> String.Format("""<section data-field="{0}">{1}</section>""", k, resolve k))
                                            |> TupleList.allkeys
                                    g |> Seq.collect (fun gd -> groupDocsHtml gd.fragments) |> String.concat "\n"
                    | StructuredText t
                        ->
                            let embedAsHtml (e:Embed) =
                                e.html |> Option.fold (fun s h ->
                                            String.Format("""<div data-oembed="{0}" data-oembed-type="{1}" data-oembed-provider="{2}">{3}</div>""",
                                                e.url, e.typ.ToLowerInvariant(), e.provider.ToLowerInvariant(), h)) String.Empty
                            let textspanAsHtml (text:string) (spans:Span seq) =
                                let writeTag (body:string) = function
                                                    | Span.Em(_, _) -> String.Format("<em>{0}</em>", body)
                                                    | Span.Strong(_, _) -> String.Format("<strong>{0}</strong>", body)
                                                    | Span.Label(_, _, label) -> String.Format("""<span class="{0}">{1}</span>""", label, body)
                                                    | Span.Hyperlink(_, _, Link.DocumentLink(l))
                                                        -> String.Format("""<a href="{0}">{1}</a>""", linkResolver l, body)
                                                    | Span.Hyperlink(_, _, Link.MediaLink(l))
                                                        -> String.Format("""<a href="{0}">{1}</a>""", l.url, body)
                                                    | Span.Hyperlink(_, _, Link.WebLink(l))
                                                        -> String.Format("""<a href="{0}">{1}</a>""", l.url, body)
                                                    | Span.Hyperlink(_, _, Link.ImageLink(l))
                                                        -> String.Format("""<a href="{0}">{1}</a>""", l.url, body)
                                //let writeHtml endingsToApply startingsToApply =
                                //    let e = endingsToApply
                                //            |> Seq.map (fun e -> writeTag false e)
                                //            |> String.concat "\n"
                                //    let s = startingsToApply
                                //            |> Seq.map (fun e -> writeTag true e)
                                //            |> String.concat "\n"
                                //    String.Format("{0}{1}", e, s)
                                let spanStart = function Span.Em(start, _) | Span.Strong(start, _) | Span.Hyperlink(start, _, _) | Span.Label(start, _, _) -> start
                                let spanEnd = function Span.Em(_, end') | Span.Strong(_, end') | Span.Hyperlink(_, end', _) | Span.Label(_, end', _) -> end'
                                let spanLength = function Span.Em(start, end') | Span.Strong(start, end') | Span.Hyperlink(start, end', _) | Span.Label(start, end', _) -> end' - start
                                let rec step in' (spans:Span list) (stack:OpenSpan list) (html:string) =
                                    match in' with
                                        | (_, pos) :: tail when (stack |> List.length > 0) && (spanEnd (stack |> List.head).span = pos) ->
                                            // Need to close a tag
                                            let tag = stack |> List.head
                                            let tagHtml = writeTag tag.spanContent tag.span
                                            let stackTail = stack |> List.tail
                                            match stackTail with
                                                | h :: t -> step in' spans ({ span=h.span; spanContent=h.spanContent + tagHtml} :: t) html
                                                | [] -> step in' spans stackTail (html + tagHtml)
                                        | (_, pos) :: tail when (spans |> List.length > 0) && (spanStart (spans |> List.head) = pos) ->
                                            // Need to open a tag
                                            let h = spans |> List.head
                                            let t = spans |> List.tail
                                            step in' t ({ span=h; spanContent=""} :: stack) html
                                        | (current, pos) :: tail ->
                                            let encoded = htmlEncode(current.ToString())
                                            match stack with
                                                | [] -> step tail spans stack (html + encoded)
                                                | h :: t -> step tail spans ({span=h.span; spanContent=h.spanContent + encoded} :: t) html
                                        | [] -> html
                                let inText = text.ToCharArray() |> Array.mapi (fun i t -> (t, i)) |> Array.toList
                                // This works because Seq.sortBy is stable, e.g. spans with the same spanStart will retain their original ordering (by length)
                                step inText (spans |> Seq.sortBy spanLength |> Seq.sortBy spanStart |> Seq.toList) List.Empty ""
                            let classCode (label: string option) =
                                match label with
                                    | Some(l) -> String.Format(" class=\"{0}\"", l)
                                    | _ -> ""
                            let blockAsHtml = function
                                    | Block.Text(Text.Heading(text, spans, level, label)) ->
                                        String.Format("""<h{0}{1}>{2}</h{0}>""", level, classCode label, textspanAsHtml text spans)
                                    | Block.Text(Text.Paragraph(text, spans, label)) ->
                                        String.Format("""<p{0}>{1}</p>""", classCode label, textspanAsHtml text spans)
                                    | Block.Text(Text.Preformatted(text, spans, label)) ->
                                        String.Format("""<pre{0}>{1}</pre>""", classCode label, textspanAsHtml text spans)
                                    | Block.Text(Text.ListItem(text, spans, _, label)) ->
                                        String.Format("""<li{0}>{1}</li>""", classCode label, textspanAsHtml text spans)
                                    | Block.Image(view) ->
                                        String.Format("""<p class="block-img">{0}</p>""", imageViewAsHtml linkResolver view)
                                    | Block.Embed(embed) -> embedAsHtml embed

                            t   |> Seq.toList |> List.fold (fun s b ->
                                    match (s, b) with
                                        | (GroupTags(Some("ul"), _) :: _ & GroupTags(ul, blocks) :: tail & group :: _,
                                            Block.Text(Text.ListItem(_, _, false, _))&block) ->  GroupTags(ul, blocks @ [block]) :: tail
                                        | (GroupTags(Some("ol"), _) :: _ & GroupTags(ul, blocks) :: tail & group :: _,
                                            Block.Text(Text.ListItem(_, _, true, _))&block) -> GroupTags(ul, blocks @ [block]) :: tail
                                        | (groups,
                                            Block.Text(Text.ListItem(_, _, false, _))&block) -> GroupTags(Some("ul"), [block]) :: groups
                                        | (groups,
                                            Block.Text(Text.ListItem(_, _, true, _))&block) -> GroupTags(Some("ol"), [block]) :: groups
                                        | (groups, block) -> GroupTags(None, [block]) :: groups) List.Empty
                                    |> List.rev
                                |> List.collect (function GroupTags(Some(tag), blocks)
                                                                -> [String.Format("<{0}>", tag)]
                                                                              @ (blocks |> List.map (fun b -> blockAsHtml b))
                                                                              @ [String.Format("</{0}>", tag)]  // @ to improve
                                                        | GroupTags(None, blocks)
                                                                ->  (blocks |> List.map (fun b -> blockAsHtml b)))
                                |> String.concat "\n"


    let getHtml linkResolver htmlSerializer field fragmentMap =
        get field fragmentMap
            |> Option.bind (fun f -> Some(asHtml linkResolver htmlSerializer f))
