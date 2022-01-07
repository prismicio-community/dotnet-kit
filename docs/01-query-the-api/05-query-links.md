# Query Links with .NET

## Query by the ID in a Link field

The following example shows how to query by a Link field. This will return any document of the type "blog-post" in which the Link field "author" has the given "author" document ID.

```cs
Response response = api.Query(
    Predicates.At("document.type", "blog-post"),
    Predicates.At("my.blog-post.author", authorDocId)
).Submit();
```

## Retrieving Additional Information

To prevent having to perform multiple queries, you can use fetchLinks to retrieve fields from the linked documents. It can only be used on simple types such as Text, Date, Timestamp and Number. StructuredText are also supported but only the first bloc will be returned, as a Text type.

```cs
// Use fetchLinks to request additional fields in linked documents
Response response = api.Query(
    Predicates.At("document.type", "blog-post"),
    Predicates.At("my.blog-post.author", authorDocId)
).FetchLinks("author.name").Submit();

Document document = response.Results.First;

// Query linked document like a top-level document
DocumentLink author = document.GetLink("blog-post.author");
String authorName = author.GetText("author.name").Value;
```
