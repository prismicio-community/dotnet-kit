# Date Templating with .NET

## Output as text

Here is a simple example that hows how to add a Date field to your templates.

```cs
<input type="text" value="@Model.document.GetDate("blog-post.date").AsText()}"/>
```
