namespace prismic
open FSharp.Data
open FSharp.Data.JsonExtensions
open System
open Shortcuts

module Fragments =

    /// fragments types

    type ImageView = { url: string; width: int; height: int; alt: string option}
                        member x.ImageRatio = x.width / x.height
    and DocumentLink = { id: string; typ: string; tags: string seq; slug: string; isBroken: bool }
    and WebLink = { url:string; contentType:string option}
    and MediaLink = { url:string; kind:string; size:Int64; filename:string }
    and GroupDoc = { fragments: Map<string, Fragment> }
    and Link = WebLink of WebLink
                | MediaLink of MediaLink
                | DocumentLink of DocumentLink
    and Span = Em of (int * int)
                | Strong of (int * int)
                | Hyperlink of (int * int * Link)
    and Embed = {typ:string; provider:string; url:string; width:int option; height:int option; html:string option; oembedJson:JsonValue}
    and Text = Heading of (string * Span seq * int) // (text, spans, level)
                | Paragraph of (string * Span seq) // (text, spans)
                | Preformatted of (string * Span seq) // (text, spans)
                | ListItem  of (string * Span seq * bool)  // (text, spans, ordered)
    and Block = Text of Text
                | Image of ImageView
                | Embed of Embed
    and StructuredText = Span of Span
                         | Block of Block
    and Color = { hex:string }
                    member x.asRGB = match tryParseHexColor x.hex with
                                        Some(r :: g :: b :: []) -> (System.Int16.Parse(r), System.Int16.Parse(g), System.Int16.Parse(b))
                                        | _ -> (0s, 0s, 0s)
    and Fragment =  | Link of Link
                    | Text of string
                    | Date of DateTime
                    | Number of float // or double ?
                    | Color of Color
                    | Embed of Embed 
                    | Image of (ImageView * Map<string, ImageView>) // (main * views)
                    | Group of GroupDoc seq
                    | StructuredText of Block seq


