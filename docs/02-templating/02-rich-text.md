# Rich Text Templating with .NET

## Output as HTML

Since Rich Text is used for your text needs, we've included an `.AsHtml` method to transform it into HTML.

The following example displays the "title" field of the blog post.

```
@Html.Raw(Model.document.GetStructuredText("blog-post.title").AsHtml(Model.Resolver))
```

This will display the rich text contained in the "body" field of the blog post.

```
@Html.Raw(Model.document.GetStructuredText("blog-post.body").AsHtml(Model.Resolver))
```

And finally this will display the "quote" field.

```
@Html.Raw(Model.document.GetStructuredText("blog-post.quote").AsHtml(Model.Resolver))
```

## Customize the HTML output

You can customize the HTML output by passing an HTML serializer to the method as shown below.

```
// In your controller
var serializer = prismic.HtmlSerializer.For ((elt, body) => {
  if (elt is fragments.StructuredText.Hyperlink) {
    var link = ((fragments.StructuredText.Hyperlink)elt).Link;
    if (link is fragments.DocumentLink) {
      var doclink = ((fragments.DocumentLink)link);
      return String.Format("<a class=\"some-link\" href=\"{0}\">{1}</a>", resolver.Resolve(doclink), body);
    }
  }
}

// In Razor
@Html.Raw(Model.document.GetStructuredText("blog-post.body").AsHtml(Model.Resolver, Model.Serializer))
```
