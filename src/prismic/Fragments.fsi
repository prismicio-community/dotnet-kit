namespace prismic
open System
open FSharp.Data

module Fragments =

    type ImageView = {url: string; width: int; height: int; alt: string option}
    and DocumentLink =
      { id: string;
       typ: string;
       tags: seq<string>;
       slug: string;
       isBroken: bool; }
    and WebLink = { url:string; contentType:string option}
    and MediaLink = { url:string; kind:string; size:Int64; filename:string }
    and GroupDoc = { fragments: Map<string,Fragment>}
    and Link =
      | WebLink of WebLink
      | MediaLink of MediaLink
      | DocumentLink of DocumentLink
    and Span =
      | Em of (int * int)
      | Strong of (int * int)
      | Hyperlink of (int * int * Link)
    and Embed =
      { typ:string; provider:string; url:string; width:int option; height:int option; html:string option; oembedJson:JsonValue }
    and Text =
      | Heading of (string * seq<Span> * int)
      | Paragraph of (string * seq<Span>)
      | Preformatted of (string * seq<Span>)
      | ListItem of (string * seq<Span> * bool)
    and Block =
      | Text of Text
      | Image of ImageView
      | Embed of Embed
    and StructuredText =
      | Span of Span
      | Block of Block
    and Fragment =
      | Link of Link
      | Text of string
      | Date of System.DateTime
      | Number of float
      | Color of string
      | Embed of Embed
      | Image of (ImageView * Map<string,ImageView>)
      | Group of seq<GroupDoc>
      | StructuredText of seq<Block>
