# Fulltext Search with .NET

## Perform a full-text search

All text content is indexed when you perform a full-text search query.

> Note that the fulltext search is not case sensitive.

Here is an example that searches for terms in all the "blog-post" documents.

```cs
var response = await api.query(
    Predicates.At("document.type", "blog-post"),
    Predicates.Fulltext("document", terms) // terms is a String
).Submit();
IList<Document> documents = response.getResults();
```
