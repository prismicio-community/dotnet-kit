# Image Templating with .NET

## Retrieve the image url

Integrating an image is done by simply retrieving its URL and setting it in the template.

The following integrates a page's illustration image field.

```cs
<!-- URL of the main view -->
<img src="@Model.document.GetImage("page.illustration").Url"/>
```

## An image & a caption field

Integrating the illustration with caption is also as straightforward:

```cs
<img src="@Model.document.GetImage("page.illustration").Url"/>
<span class="image-caption">@Model.document.GetText("page.caption").AsText()}</span>
```

## A group of images

For integrating a group of images (e.g. photo slide show in a page), you need to loop through the items:

```cs
@foreach (var imageWithCaption in Model.document.GetGroup("page.slide-show").GetDocs()) {
  <img src="@imageWithCaption.GetImage("page.illustration").Url"/>
  <span class="image-caption">@imageWithCaption.GetText("page.caption").AsText()</span>
}
```
