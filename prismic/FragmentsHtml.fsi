namespace prismic
open System

/// Signature file for the FragmentsHtml module
module FragmentsHtml =

    val asHtml : linkResolver:(Fragments.DocumentLink->string) -> fragment:Fragments.Fragment -> string

    val imageViewAsHtml : imageView:Fragments.ImageView -> string