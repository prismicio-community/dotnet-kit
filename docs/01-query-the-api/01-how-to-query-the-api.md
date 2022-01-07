# Query the API

A productive way to build pages with Prismic is to follow an iterative cycle of **defining** fields for your Custom Types, **querying** its content, and **integrating** it into templates.

Queries allow you to retrieve content to be displayed on your pages.

## Query predicates

Queries in Prismic are based on predicates. Some predicates apply to any Custom Type, and some are field-specific. You can also combine multiple predicates to express your query.

Here is an example query for retrieving a basic page through its UID:

```cs
// String uid comes from the querystring
Document document = api.GetByUID("basic-page", uid);
```

The following example retrieves all blog posts:

```cs
Response response = api.query(Predicates.At("document.type", "blog-post"))
   .PageSize(10)
   .Orderings("my.blog-post.date desc")
   .Submit();
```

A full-text search on all blog posts is done combining two predicates:

```cs
Response response = api.Query(
    Predicates.At("document.type", "blog-post"),
    Predicates.Fulltext("document", terms) // terms is a String
).Submit();
IList<Document> documents = response.Results;
```

> **Pagination of API Results**
>
> When querying a Prismic repository, your results will be paginated. By default, there are 20 documents per page in the results. You can read more about how to manipulate the pagination below with the query options.

## Predicates reference

### At(path, value)

The `At` operator is the equality operator, checking that the fragment matches the described value exactly. <br/>It takes a value for a field or an array (only for tags).

| Property                       | Description                                                    |
| ------------------------------ | -------------------------------------------------------------- |
| <strong>path</strong><br/>     | <p>document.type<br />document.tags<br />my.{type}.{field}</p> |
| <strong>value</strong><br/>    | <p>single value</p>                                            |
| <strong>examples</strong><br/> | <pre>At(&quot;document.type&quot;, &quot;product&quot;)        |

At(&quot;document.tags&quot;, [&quot;Macaron&quot;, &quot;Cupcake&quot;])
At(&quot;my.articles.gender&quot;, &quot;male&quot;)</pre>|

### Not(path, value)

The `Not` operator is the different operator, checking that the fragment doesn't match the described value exactly. <br/>It takes a value for a field.

| Property                      | Description                                                    |
| ----------------------------- | -------------------------------------------------------------- |
| <strong>path</strong><br/>    | <p>document.type<br />my.{type}.{field}</p>                    |
| <strong>value</strong><br/>   | <p>single value</p>                                            |
| <strong>example</strong><br/> | <pre>Not(&quot;document.type&quot;, &quot;product&quot;)</pre> |

### Any(path, values)

The `Any` operator takes an array of strings as a value. It works exactly the same way as the at operator, but checks whether the fragment matches either one of the values in the array. You can use it with all fragment types.

| Property                      | Description                                                                             |
| ----------------------------- | --------------------------------------------------------------------------------------- |
| <strong>path</strong><br/>    | <p>document.type</p><p>my.{type}.{field}</p>                                            |
| <strong>values</strong><br/>  | <p>array of values</p>                                                                  |
| <strong>example</strong><br/> | <pre>Any(&quot;document.type&quot;, [&quot;product&quot;, &quot;blog-post&quot;])</pre> |

### In(path, values)

The `In`\*\* \*\*operator is used to retrieve documents using an Array of IDs or UIDs. It returns the documents in the same order as the passed Array.

| Property                      | Description                                                                 |
| ----------------------------- | --------------------------------------------------------------------------- |
| <strong>path</strong><br/>    | <p>document.type</p><p>my.{type}.{field}</p>                                |
| <strong>values</strong><br/>  | <p>array of values</p>                                                      |
| <strong>example</strong><br/> | <pre>In(&quot;document.uid, [&quot;myuid1&quot;, &quot;myuid2&quot;])</pre> |

### Fulltext(path, value)

The `Fulltext` operator provides two capabilities: either you want to check if a certain string is anywhere inside a document (this is what you should use to make your project's search engine feature), or if the string is contained inside a specific document's structured text fragment. In the example, you want to search the term `music` in the document

If you want to check in a given structured text fragment, you will use something like the second example.

| Property                                                        | Description                                            |
| --------------------------------------------------------------- | ------------------------------------------------------ |
| <strong>path</strong><br/>                                      | <p>document</p><p>my.{type}.{field}</p>                |
| <strong>value</strong><br/>                                     | <p>search terms</p>                                    |
| <strong>example</strong><br/>                                   | <pre>Fulltext(&quot;document&quot;, &quot;music&quot;) |
| Fulltext(&quot;my.product.title&quot;, &quot;music&quot;)</pre> |

### Has(path)

The `Has` operator check whether a fragment has a value.

| Property                      | Description                                |
| ----------------------------- | ------------------------------------------ |
| <strong>path</strong><br/>    | <p>my.{type}.{field}</p>                   |
| <strong>example</strong><br/> | <pre>Has(&#39;my.product.price&#39;)</pre> |

### Missing(path)

The `Missing` operator check if a fragment doesn't have a value. Note that the missing operator will restrict the results to the type implied in the fragment path.

| Property                      | Description                                    |
| ----------------------------- | ---------------------------------------------- |
| <strong>path</strong><br/>    | <p>my.{type}.{field}</p>                       |
| <strong>example</strong><br/> | <pre>Missing(&#39;my.product.price&#39;)</pre> |

### Similar(id,value)

The `Similar` operator takes the ID of a document and returns a list of documents whose contents are similar. This allows to build an automated content discovery feature (for instance, a "Related posts" block) at almost no cost.

| Property                          | Description                                                                                     |
| --------------------------------- | ----------------------------------------------------------------------------------------------- |
| <strong>id</strong><br/>          | <p>document ID</p>                                                                              |
| <strong>value</strong><br/>number | <p>the maximum count of documents that a term may appear in to be still considered relevant</p> |
| <strong>example</strong><br/>     | <pre>similar(&quot;VkRmhykAAFA6PoBj&quot;, 10)</pre>                                            |

## Predicate options

### PageSize(value)

The PageSize options define the maximum of documents that the API will return for your query. Default is 20, max is 100.

| Property                    | Description      |
| --------------------------- | ---------------- |
| <strong>value</strong><br/> | <p>page size</p> |
|                             | <p></p>          |

### Page(value)

The page options define the pagination for the result of your query. Defaults to "1", corresponding to the first page.

| Property                    | Description       |
| --------------------------- | ----------------- |
| <strong>value</strong><br/> | <p>page index</p> |
|                             | <p></p>           |

### Orderings(values)

Order result by fields. You can specify as many fields as you want, in order to address all of the documents you are querying or if you have the same value on a given field. Use "desc" next to the field name, to order it from greatest to lowest.

| Property                     | Description   |
| ---------------------------- | ------------- |
| <strong>values</strong><br/> | <p>fields</p> |
|                              | <p></p>       |

### After(value)

Query documents after a specific document. By reversing the ordering, you can query for previous documents. Useful when creating navigation for a blog.

| Property                    | Description        |
| --------------------------- | ------------------ |
| <strong>value</strong><br/> | <p>document id</p> |
|                             | <p></p>            |

### FetchLinks(values)

Additional fields to retrieve in LinkDocument fragments. The LinkDocument can then be queried like a Document. Note that this only works with basic fields such as Text, Number or Date. It is not possible to retrieve StructuredText fragments from linked document using this field.

| Property                    | Description   |
| --------------------------- | ------------- |
| <strong>value</strong><br/> | <p>fields</p> |
|                             | <p></p>       |

### Fetch(values)

fetch only specific fields

| Property                    | Description               |
| --------------------------- | ------------------------- |
| <strong>value</strong><br/> | <p>fields to retrieve</p> |
|                             | <p></p>                   |
