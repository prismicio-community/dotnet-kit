namespace prismic
open System
open FSharp.Data

/// Signature file for the Fragment module
module Fragments =

    type ImageView = {url: string; width: int; height: int; alt: string option; linkTo: Link option }
                        with member ImageRatio : int end
    and DocumentLink =
      { id: string;
       typ: string;
       tags: seq<string>;
       slug: string;
       isBroken: bool; }
    and WebLink = { url:string; contentType:string option}
    and MediaLink = { url:string; kind:string; size:Int64; filename:string }
    and ImageLink = { name:string; url:string; size:Int64; height:Int64; width:Int64 }
    and GroupDoc = { fragments: TupleList<string, Fragment> }
    and Link =
      | WebLink of WebLink
      | MediaLink of MediaLink
      | DocumentLink of DocumentLink
      | ImageLink of ImageLink
    and Span =
      | Em of (int * int) // start, end
      | Strong of (int * int) // start, end
      | Hyperlink of (int * int * Link) // start, end, link
      | Label of (int * int * string) // start, end, label
    and Embed =
      { typ:string; provider:string; url:string; width:int option; height:int option; html:string option; oembedJson:JsonValue; label: string option }
    and GeoPoint = {latitude: decimal; longitude: decimal}
    and Text =
      | Heading of (string * seq<Span> * int * string option) // text, spans, level, label
      | Paragraph of (string * seq<Span> * string option) // text, spans, label
      | Preformatted of (string * seq<Span> * string option)  // text, spans, label
      | ListItem of (string * seq<Span> * bool * string option)  // text, spans, ordered, label
    and Block =
      | Text of Text
      | Image of ImageView
      | Embed of Embed
    and StructuredText =
      | Span of Span
      | Block of Block
    and Color =
        { hex:string } with member asRGB : Int16 * Int16 * Int16 end
    and Fragment =
      | Link of Link
      | Text of string
      | Date of System.DateTime
      | Timestamp of System.DateTime
      | Number of float
      | Color of Color
      | Embed of Embed
      | GeoPoint of GeoPoint
      | Image of (ImageView * TupleList<string, ImageView>) // main view, views
      | Group of seq<GroupDoc>
      | StructuredText of seq<Block>
