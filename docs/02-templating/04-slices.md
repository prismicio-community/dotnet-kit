# Slices Templating with .NET

## Looping through slices

Here is an example of integrating slices into a rich blog post.

```cs
@foreach (var slice in document.GetSliceZone("blog-post.body").Value) {
  //- Render the right markup for a given slice type.
  @switch (slice.SliceType) {
    case "text": @: <div>@slice.Value.AsHtml(linkResolver)}</div>
    break;
    case "quote": @: <span class='block-quotation'>slice.Value</span>
    break;
    case "image-with-caption": @:
      <p class='block-img'><img src="@slice.Value.Get("illustration").Url"></p>
      <span class='image-label'>@slice.Value.get("caption").AsText()</span>
}
```
