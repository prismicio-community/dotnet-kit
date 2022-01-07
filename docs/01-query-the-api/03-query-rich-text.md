# Query Rich Text with .NET

## Retrieve content with full-text search

Rich Text can be queried with a full-text predicate. Here is a full-text query on content of type "blog-post".

```cs
var response = await api.Query(
    Predicates.At("document.type", "blog-post"),
    Predicates.Fulltext("document", terms)
).Submit();
```

## Orderings

Structured text fields can be used for orderings. Only the first text block will be used.

```cs
var response = await api.Query(
    Predicates.At("document.type", "blog-post"),
    Predicates.Fulltext("document", terms)
).Orderings("my.blog-post.title").Submit();
```

## Retrieving Additional Information

To prevent having to perform multiple queries, you can use fetchLinks to retrieve fields from the linked documents. It can only be used on simple types such as Text, Date, Timestamp and Number.

It works for links within StructuredText just like link fragments. Usage is detailed on the [Query Links](../01-query-the-api/05-query-links.md) page.
