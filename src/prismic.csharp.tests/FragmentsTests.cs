using NUnit.Framework;
using prismic;
using prismic.extensions;
using System;
using System.Linq;
using Microsoft.FSharp.Core;

namespace prismic.csharp.tests
{
	[TestFixture ()]
	public class FragmentsTests
	{
		[Test ()]
		public void ShouldAccessGroupField()
		{
			var url = "https://micro.prismic.io/api";
			Api.Api api = (prismic.extensions.Api.Get(url, new prismic.Infra.NoCache<prismic.Api.Response>(), (l, m) => {})).Result;
			var form = api.Forms["everything"].Ref(api.Master).Query (@"[[:d = at(document.type, ""docchapter"")]]").SubmitableAsTask();

			var document = form.Submit().Result.results.First();
			var maybeGroup = document.GetGroup ("docchapter.docs");
			Assert.IsTrue (maybeGroup.Exists(), "group was not found");

			var maybeFirstDoc = maybeGroup.Value.Item.FirstOrDefault (); 
			Assert.IsNotNull (maybeFirstDoc, "doc was not found");

			var maybeLink = maybeFirstDoc.GetLink ("linktodoc");
			Assert.IsTrue (maybeLink.Exists(), "link was not found");
		}

		[Test ()]
		public void ShouldSerializeGroupToHTML()
		{
			var url = "https://micro.prismic.io/api";
			Api.Api api = (prismic.extensions.Api.Get(url, new prismic.Infra.NoCache<prismic.Api.Response>(), (l, m) => {})).Result;
			var form = api.Forms["everything"].Ref(api.Master).Query (@"[[:d = at(document.type, ""docchapter"")]]").SubmitableAsTask();

			var document = form.Submit().Result.results.ElementAt(1);
			var maybeGroup = document.GetGroup ("docchapter.docs");

			Assert.IsTrue (maybeGroup.Exists(), "group was not found");

			var resolver = 
				prismic.extensions.DocumentLinkResolver.For (l => String.Format ("http://localhost/{0}/{1}", l.typ, l.id));

			var html = maybeGroup.BindAsHtml(resolver);
			Assert.IsTrue (html.Exists ());
			Assert.AreEqual(@"<section data-field=""desc""><p>Just testing another field in a group section.</p></section>
<section data-field=""linktodoc""><a href=""http://localhost/doc/UrDejAEAAFwMyrW9"">installing-meta-micro</a></section>
<section data-field=""linktodoc""><a href=""http://localhost/doc/UrDmKgEAALwMyrXA"">using-meta-micro</a></section>", html.Value);
		}
			

		[Test ()]
		public void ShouldAccessMediaLink()
		{
			var url = "https://test-public.prismic.io/api";
			Api.Api api = (prismic.extensions.Api.Get(url, new prismic.Infra.NoCache<prismic.Api.Response>(), (l, m) => {})).Result;
			var form = api.Forms["everything"].Ref(api.Master).Query (@"[[:d = at(document.id, ""Uyr9_wEAAKYARDMV"")]]").SubmitableAsTask();

			var document = form.Submit().Result.results.First();
			var maybeLink = document.GetLink ("test-link.related");
			Assert.IsTrue (maybeLink.Exists(), "link was not found");
			Assert.AreEqual ("baastad.pdf", maybeLink.BindAsMediaLink ().Value.filename);

		}

		[Test ()]
		public void ShouldAccessFirstLinkInMultipleDocumentLink()
		{
			var url = "https://lesbonneschoses.prismic.io/api";
			Api.Api api = (prismic.extensions.Api.Get(url, new prismic.Infra.NoCache<prismic.Api.Response>(), (l, m) => {})).Result;
			var form = api.Forms["everything"].Ref(api.Master).Query (@"[[:d = at(document.id, ""UkL0gMuvzYUANCpi"")]]").SubmitableAsTask();

			var document = form.Submit().Result.results.First();
			var maybeLink = document.GetLink ("job-offer.location");
			Assert.IsTrue (maybeLink.Exists(), "link was not found");
			Assert.AreEqual ("new-york-fifth-avenue", maybeLink.BindAsDocumentLink ().Value.slug);
		}

		[Test ()]
		public void ShouldFindAllLinksInMultipleDocumentLink()
		{
			var url = "https://lesbonneschoses.prismic.io/api";
			Api.Api api = (prismic.extensions.Api.Get(url, new prismic.Infra.NoCache<prismic.Api.Response>(), (l, m) => {})).Result;
			var form = api.Forms["everything"].Ref(api.Master).Query (@"[[:d = at(document.id, ""UkL0gMuvzYUANCpi"")]]").SubmitableAsTask();

			var document = form.Submit().Result.results.First();
			var links = document.GetAll ("job-offer.location");
			Assert.AreEqual (5, links.Count());
			Assert.AreEqual ("new-york-fifth-avenue", FSharpOption<Fragments.Fragment>.Some(links.ElementAt(0)).BindAsDocumentLink ().Value.slug);
			Assert.AreEqual ("tokyo-roppongi-hills", FSharpOption<Fragments.Fragment>.Some(links.ElementAt(1)).BindAsDocumentLink ().Value.slug);
		}

		[Test ()]
		public void ShouldAccessStructuredText()
		{
			var url = "https://lesbonneschoses.prismic.io/api";
			Api.Api api = (prismic.extensions.Api.Get(url, new prismic.Infra.NoCache<prismic.Api.Response>(), (l, m) => {})).Result;
			var form = api.Forms["everything"].Ref(api.Master).Query (@"[[:d = at(document.id, ""UkL0gMuvzYUANCpu"")]]").SubmitableAsTask();

			var document = form.Submit().Result.results.First();
			var maybeText = document.GetStructuredText ("article.content");
			Assert.IsTrue (maybeText.Exists ());
		}


		[Test ()]
		public void ShouldAccessImage()
		{
			var url = "https://test-public.prismic.io/api";
			Api.Api api = (prismic.extensions.Api.Get(url, new prismic.Infra.NoCache<prismic.Api.Response>(), (l, m) => {})).Result;
			var form = api.Forms["everything"].Ref(api.Master).Query (@"[[:d = at(document.id, ""Uyr9sgEAAGVHNoFZ"")]]").SubmitableAsTask();

			var document = form.Submit().Result.results.First();
			var maybeImgView = document.GetImageView ("article.illustration", "icon");
			Assert.IsTrue (maybeImgView.Exists ());

			var html = maybeImgView.BindAsHtml().Value;

			var someurl = "https://prismic-io.s3.amazonaws.com/test-public/9f5f4e8a5d95c7259108e9cfdde953b5e60dcbb6.jpg";
			var expect = String.Format (@"<img alt=""some alt text"" src=""{0}"" width=""100"" height=""100"" />", someurl);

			Assert.AreEqual(expect, html);
		}


	}
}

