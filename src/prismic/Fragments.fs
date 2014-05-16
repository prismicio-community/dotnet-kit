namespace prismic
open FSharp.Data
open FSharp.Data.JsonExtensions
open System
open ShortcutsAndUtils

module Fragments =
    
    /// fragments types

    type ImageView = (string * int * int * string option) // (url, width, height, alt)
    and DocumentLink = { id: string; typ: string; tags: string seq; slug: string; isBroken: bool }
    and GroupDoc = Map<string, Fragment>
    and Link = WebLink of (string * string option) // (url, contentType)
                | MediaLink of (string * string * Int64 * string) // (url, kind, size, filename)
                | DocumentLink of DocumentLink
    and Span = Em of (int * int)
                | Strong of (int * int)
                | Hyperlink of (int * int * Link)
    and Embed = (string * string * string * int option * int option * string option * JsonValue) // typ, provider, url, width, height, html, oembedJson
    and Text = Heading of (string * Span seq * int) // (text, spans, level)
                | Paragraph of (string * Span seq) // (text, spans)
                | Preformatted of (string * Span seq) // (text, spans)
                | ListItem  of (string * Span seq * bool)  // (text, spans, ordered)
    and Block = Text of Text
                | Image of ImageView
                | Embed of Embed
    and StructuredText = Span of Span
                         | Block of Block
    and Fragment =  | Link of Link
                    | Text of string
                    | Date of DateTime
                    | Number of float // or double ?
                    | Color of string
                    | Embed of Embed 
                    | Image of (ImageView * Map<string, ImageView>) // (main * views)
                    | Group of GroupDoc seq
                    | StructuredText of Block seq


