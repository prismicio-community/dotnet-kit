# Query by UID with .NET

This page shows how to query for a certain document by its UID.

## Using the GetByUID Method

The UID is unique within a custom type; documents of different types can have the same UID. This is why we add the API ID value of "page" below.

```cs
// UID is a string
Document document = await api.GetByUID("page", uid);
```

## Preventing duplicate content

When querying a document by UID, older values will also return your document. This ensure that existing links are not broken when the UID is changed.

For better search engines ranking (SEO), you may want to redirect old URLs to the latest URL rather than serving the same content on both old and new:

```cs
// Using MVC.NET
public async Task<ActionResult> Detail(string uid)
{
    var api = await prismic.Api.Get(endpoint);
    var document = await api.GetByUID("page", uid);
    if (document == null) { // 404
        return new HttpNotFoundResult("Page not found");
    } else if (document.Uid != uid) { // 301
        return RedirectToActionPermanent ("Detail", new { document.Uid });
    } else { // 200
        return View (new PrismicDocument (document));
    }
}
```
