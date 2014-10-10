using NUnit.Framework;
using prismic;
using prismic.extensions;
using System;
using System.Linq;
using System.ComponentModel;
using Microsoft.FSharp.Core;

namespace prismic.csharp.tests
{
	[TestFixture ()]
	public class DocTest
	{
		[Test ()]
		public void ApiTest ()
		{
			// startgist:c023234afbc20303f792:prismic-api.cs
			Api.Api api = prismic.extensions.Api.Get("https://lesbonneschoses.prismic.io/api").Result;
			// endgist
			Assert.IsNotNull (api);
		}

		[Test ()]
		public void SimpleQueryTest ()
		{
			// startgist:6b01f5bd50568045f9a0:prismic-simplequery.cs
			Api.Api api = prismic.extensions.Api.Get("https://lesbonneschoses.prismic.io/api").Result;
			var response = api
				.Forms["everything"]
				.Ref(api.Master)
				.Query (@"[[:d = at(document.type, ""product"")]]")
				.SubmitableAsTask().Submit().Result;
			// The response object contains all documents of type "product", paginated
			// endgist
			Assert.AreEqual (16, response.results.Count());
		}

		[Test ()]
		public void PredicatesTest ()
		{
			// startgist:dbd1a1f4056ae7bf9959:prismic-predicates.cs
			Api.Api api = prismic.extensions.Api.Get("https://lesbonneschoses.prismic.io/api").Result;
			var response = api
				.Forms["everything"]
				.Ref(api.Master)
				.Query (@"[[:d = at(document.type, ""blog-post"")][:d = date.after(my.blog-post.date, 1401580800000)]]")
				.SubmitableAsTask().Submit().Result;
			// endgist
			Assert.AreEqual (0, response.results.Count());
		}

		[Test ()]
		public void AsHtmlTest ()
		{
			Api.Api api = prismic.extensions.Api.Get("https://lesbonneschoses.prismic.io/api").Result;
			var response = api
				.Forms["everything"]
				.Ref(api.Master)
				.Query (@"[[:d = at(document.id, ""UlfoxUnM0wkXYXbX"")]]")
				.SubmitableAsTask().Submit().Result;
			// startgist:097067bd2495233520bb:prismic-asHtml.cs
			var document = response.results.First ();
			var resolver =
				prismic.extensions.DocumentLinkResolver.For (l => String.Format ("http://localhost/{0}/{1}", l.typ, l.id));
			var html = document.GetStructuredText ("blog-post.body").BindAsHtml(resolver);
			// endgist
			Assert.IsTrue (html.Exists ());
		}

		[Test ()]
		public void HtmlSerializerTest ()
		{
			Api.Api api = prismic.extensions.Api.Get("https://lesbonneschoses.prismic.io/api").Result;
			var response = api
				.Forms["everything"]
				.Ref(api.Master)
				.Query (@"[[:d = at(document.id, ""UlfoxUnM0wkXYXbX"")]]")
				.SubmitableAsTask().Submit().Result;
			// startgist:b5f2de0fb813b52a14a9:prismic-htmlSerializer.cs
			var document = response.results.First ();
			var resolver =
				prismic.extensions.DocumentLinkResolver.For (l => String.Format ("http://localhost/{0}/{1}", l.typ, l.id));
			var serializer = prismic.extensions.HtmlSerializer.For ((elt, body) => {
				if (elt is Fragments.Span.Hyperlink) {
					// Add a class to hyperlinks
					var link = ((Fragments.Span.Hyperlink)elt).Item.Item3;
					if (link is Fragments.Link.DocumentLink) {
						var doclink = ((Fragments.Link.DocumentLink)link).Item;
						return String.Format("<a class=\"some-link\" href=\"{0}\">{1}</a>", resolver.Apply(doclink), body);
					}
				}
				if (elt is Fragments.Block.Image) {
					// Don't wrap images in <p> blocks
					var imageview = ((Fragments.Block.Image)elt).Item;
					return imageview.AsHtml(resolver);
				}
				return null;
			});
			var html = document.GetStructuredText ("blog-post.body").BindAsHtml(resolver, serializer);
			// endgist
			Assert.IsTrue (html.Exists ());
		}

	}
}

