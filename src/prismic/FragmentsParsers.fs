namespace prismic
open System
open FSharp.Data
open FSharp.Data.JsonExtensions
open ShortcutsAndUtils
open Fragments

module FragmentsParsers = 

    let tryParseHexColor = function
         | ParseRegex "^#([0-9a-fA-F]{2})([0-9a-fA-F]{2})([0-9a-fA-F]{2})$" d -> Some(d)
         | _ -> None


    let parseImageView (f:JsonValue) = {    url=  f?url.AsString(); 
                                            width= f?dimensions?width.AsInteger(); 
                                            height= f?dimensions?height.AsInteger(); 
                                            alt= asStringOption(f>?"alt") }
    let parseImage (f:JsonValue) = 
                        let main = parseImageView f?main
                        let views = asMapFromProperties parseImageView f?views
                        Image(main, views)

    let parseColor (f:JsonValue) = 
        let s = f.AsString()
        let parsed = tryParseHexColor(s) <?-- (lazy raise (ArgumentException("Invalid color value " + s)))  // err : Invalid color value $v // or parsing exception
        Color(s)
    let parseNumber (f:JsonValue) = 
        Number(
            f.AsFloat()) // err : Invalid number value $v
    let parseDate (f:JsonValue) = 
        Date(
            f.AsDateTime()) // err : Invalid date value $v
    let parseText (f:JsonValue) = 
        Text(
            f.AsString()) // err : Invalid text value $v
    let parseSelect (f:JsonValue) = 
        Text(
            f.AsString()) // err : Invalid text value $v
    let parseEmbed (f:JsonValue) = 
        let oembed = f?oembed in
        {
            typ = oembed.GetProperty("type").AsString();
            provider = oembed?provider_name.AsString();
            url = oembed?embed_url.AsString();
            width = asIntegerOption(oembed>?"width"); 
            height = asIntegerOption(oembed>?"height"); 
            html = asStringOption(oembed>?"html");
            oembedJson = oembed 
        }
    let parseFragmentEmbed (f:JsonValue) = Fragment.Embed(parseEmbed(f))
    let parseWebLink (f:JsonValue) = 
        WebLink({
                url = f?url.AsString();
                contentType = Option.None
        })
    let parseFragmentWebLink (f:JsonValue) = Fragment.Link(parseWebLink(f))
    let parseDocumentLink (f:JsonValue) = 
        let doc = f?document in
        let isBroken = (asBooleanOption(f>?"isBroken")) <?- false
        DocumentLink({
                        id = doc?id.AsString();
                        typ = doc.GetProperty("type").AsString();
                        tags = asArrayOrEmpty doc "tags" |> Array.map (fun j -> j.AsString()) |> Seq.ofArray; 
                        slug = doc?slug.AsString();
                        isBroken = isBroken})
    let parseFragmentDocumentLink (f:JsonValue) = Fragment.Link(parseDocumentLink(f))
    let parseMediaLink (f:JsonValue) = 
        let file = f?file in
        MediaLink({
            url = file?url.AsString(); 
            kind = file?kind.AsString();
            size = file?size.AsInteger64(); 
            filename = file?name.AsString()})
    let parseFragmentMediaLink (f:JsonValue) = Fragment.Link(parseMediaLink(f))
    let parseStructuredText (f:JsonValue) =  // recheck StructuredText read logic ///////
        let parseSpan (f:JsonValue) = 
            let type' = f.GetProperty("type").AsString()
            let start = f?start.AsInteger()
            let end' = f.GetProperty("end").AsInteger()
            let data = f>?"data" 
            match (type', data) with 
                        | ("strong", _) -> Strong(start, end')
                        | ("em", _) -> Em(start, end')
                        | ("hyperlink", Some(d)) when d.GetProperty("type").AsString() = "Link.web" -> 
                            Hyperlink(start, end', parseWebLink(d?value))
                        | ("hyperlink", Some(d)) when d.GetProperty("type").AsString() = "Link.document" -> 
                            Hyperlink(start, end', parseDocumentLink(d?value)) 
                        | ("hyperlink", Some(d)) when d.GetProperty("type").AsString() = "Link.file" -> 
                            Hyperlink(start, end', parseMediaLink(d?value)) 
                        | (t, _) -> raise (ArgumentException("Unsupported span type "+ t))
        let parseSpanSeq (f:JsonValue) = f?spans.AsArray() |> Array.map (fun s -> parseSpan s) |> Seq.ofArray
        let parseHeading level (f:JsonValue) = 
            Block.Text(
                Heading(
                        f?text.AsString(), 
                        parseSpanSeq f, 
                        level))
        let parseParagraph (f:JsonValue) = 
            Block.Text(
                Paragraph(
                    f?text.AsString(), 
                    parseSpanSeq f))
        let parsePreformatted (f:JsonValue) =
            Block.Text(
                Preformatted(
                    f?text.AsString(), 
                    parseSpanSeq f))
        let parseListItemWithOrdered ordered (f:JsonValue) = 
            Block.Text(
                ListItem(
                    f?text.AsString(), 
                    parseSpanSeq f,
                    ordered))
        let parseListItem = parseListItemWithOrdered false
        let parseOListItem = parseListItemWithOrdered true
        let parseBlockImage (f:JsonValue) = Block.Image(parseImageView f)
        let parseBlockEmbed (f:JsonValue) = Block.Embed(parseEmbed f)

        let parseBlock (f:JsonValue) = match f.GetProperty("type").AsString() with
                                        | "heading1" -> parseHeading 1 f
                                        | "heading2" -> parseHeading 2 f
                                        | "heading3" -> parseHeading 3 f
                                        | "heading4" -> parseHeading 4 f
                                        | "paragraph" -> parseParagraph f
                                        | "preformatted" -> parsePreformatted f
                                        | "list-item" -> parseListItem f
                                        | "o-list-item" -> parseOListItem f
                                        | "image" -> parseBlockImage f
                                        | "embed" -> parseBlockEmbed f
                                        | t -> raise (ArgumentException("Unsupported block type " + t))
        let blocks = f.AsArray() |> Array.map (fun a -> parseBlock a) |> Seq.ofArray
        StructuredText(blocks)


    let rec parseFragment (j:JsonValue) = 
        let parseGroup (f:JsonValue) = 
            let g = f.AsArray() |> Array.map (fun x -> {fragments = asMapFromOptionProperties parseFragment x}) |> Seq.ofArray
            Group(g)

        let t = j.GetProperty("type").AsString() in // NOT OK : use tryget - type is not mandatory
        let maybeParser = match t with
                            | "Image" -> Some(parseImage) 
                            | "Color" -> Some(parseColor)
                            | "Number" -> Some(parseNumber) 
                            | "Date" -> Some(parseDate) 
                            | "Text" -> Some(parseText) 
                            | "Select" -> Some(parseSelect) 
                            | "Embed" -> Some(parseFragmentEmbed)
                            | "Link.web" -> Some(parseFragmentWebLink)
                            | "Link.document" -> Some(parseFragmentDocumentLink)
                            | "Link.file" -> Some(parseFragmentMediaLink)
                            | "StructuredText" -> Some(parseStructuredText)
                            | "Group" -> Some(parseGroup)
                            | _ -> None
        let value = j.GetProperty("value") in
        maybeParser |> Option.bind (fun parser -> Some(parser value))
    
    type ParsedFieldArrayElement = (int * Fragment)
    type ParsedField = Single of Fragment
                        | Array of ParsedFieldArrayElement[]

    let parseField (j:JsonValue) = 
        match j with
            | JsonArray a ->    
                            let elements = a 
                                            |> Array.mapi (fun i o -> match parseFragment o with
                                                                                    | Some(f) -> Some(i, f)
                                                                                    | _ -> None)
                                            |> Array.choose id
                            Some(Array(elements))
            | o -> match parseFragment o with
                            | Some(f) -> Some(Single(f))
                            | _ -> None

    
    let imageRatio (view:ImageView) = view.width / view.height

    let asRGB hex = match tryParseHexColor hex with
                        Some(r :: g :: b :: []) -> (System.Int16.Parse(r), System.Int16.Parse(g), System.Int16.Parse(b))
                        | _ -> (0s, 0s, 0s)

