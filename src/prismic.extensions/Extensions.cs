using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.FSharp.Control;
using Microsoft.FSharp.Core;
using Microsoft.FSharp.Collections;
using prismic;


namespace prismic.extensions
{
	public static class Api
	{

		/// <summary>
		/// Get the API at a specified url, given a cache and logger.
		/// </summary>
		/// <param name="url">URL.</param>
		/// <param name="cache">Cache.</param>
		/// <param name="logger">Logger.</param>
		public static Task<prismic.Api.Api> Get(string url, prismic.ApiInfra.ICache<prismic.Api.Response> cache, Action<string, string> logger)
		{
			return FSharpAsync.StartAsTask (prismic.Api.get(cache, logger.ToFSharpFunc(),
				FSharpOption<string>.None, url), FSharpOption<TaskCreationOptions>.None , FSharpOption<CancellationToken>.None);
		}

		/// <summary>
		/// Get the API at a specified url, given an authentication token, a cache and logger.
		/// </summary>
		/// <param name="token">Token.</param>
		/// <param name="url">URL.</param>
		/// <param name="cache">Cache.</param>
		/// <param name="logger">Logger.</param>
		public static Task<prismic.Api.Api> Get(string token, string url, prismic.ApiInfra.ICache<prismic.Api.Response> cache, Action<string, string> logger)
		{
			return FSharpAsync.StartAsTask (prismic.Api.get(cache, logger.ToFSharpFunc(),
				FSharpOption<string>.Some(token), url), FSharpOption<TaskCreationOptions>.None , FSharpOption<CancellationToken>.None);
		}
		/// <summary>
		/// Get the API at a specified url, given an option of authentication token, a cache and logger.
		/// </summary>
		/// <param name="maybeToken">Maybe token.</param>
		/// <param name="url">URL.</param>
		/// <param name="cache">Cache.</param>
		/// <param name="logger">Logger.</param>
		public static Task<prismic.Api.Api> Get(FSharpOption<string> maybeToken, string url, prismic.ApiInfra.ICache<prismic.Api.Response> cache, Action<string, string> logger)
		{
			return FSharpAsync.StartAsTask (prismic.Api.get(cache, logger.ToFSharpFunc(), maybeToken, url), 
				FSharpOption<TaskCreationOptions>.None , FSharpOption<CancellationToken>.None);
		}

		/// <summary>
		/// Allows to use on the form a Submit method that will return a regular Task instead of an FSharpAsync
		/// </summary>
		/// <returns>The as task.</returns>
		/// <param name="form">Form.</param>
		public static SearchFormWithTask SubmitableAsTask(this prismic.Api.SearchForm form)
		{
			return new SearchFormWithTask (form);
		}

		/// <summary>
		/// Search form wrapper, for providing a Submit method that return a Task.
		/// </summary>
		public class SearchFormWithTask
		{
			private readonly prismic.Api.SearchForm form;
			internal SearchFormWithTask(prismic.Api.SearchForm form)
			{
				this.form = form;
			}
			/// <summary>
			/// Submit the form and return a Task.
			/// </summary>
			public Task<prismic.Api.Response> Submit()
			{
				return FSharpAsync.StartAsTask (form.Submit(), FSharpOption<TaskCreationOptions>.None, FSharpOption<CancellationToken>.None);
			}
		}
	}


	public static class SearchFormExt
	{
		/// <summary>
		/// Does same as Ref(ref) if ref has some value, else does not set any Ref.
		/// </summary>
		/// <returns>The reference.</returns>
		/// <param name="form">Form.</param>
		/// <param name="refValue">Reference value.</param>
		public static prismic.Api.SearchForm Ref(this prismic.Api.SearchForm form, FSharpOption<string> maybeRef)
		{
			return maybeRef.Exists() ? form.Ref (maybeRef.Value) : form;
		}
	}

	public static class DocumentLinkResolver
	{
		/// <summary>Builds a document link resolver that will apply the function to document links.</summary>
		/// <param name="f">the resolving strategy.</param>
		/// <returns>a DocumentLinkResolver for the given strategy.</returns>
		public static prismic.Api.DocumentLinkResolver For(System.Func<Fragments.DocumentLink, string> resolver)
		{
			return prismic.Api.DocumentLinkResolver.For(CSharpAdapters.CreateFunc(resolver));
		}
		/// <summary>Builds a document link resolver that will apply the function to document links and bookmark.</summary>
		/// <param name="api">the api.</param>
		/// <param name="f">the resolving strategy, on document link and evenually a bookmark.</param>
		/// <returns>a DocumentLinkResolver for the given strategy.</returns>
		public static prismic.Api.DocumentLinkResolver For(prismic.Api.Api api, System.Func<Fragments.DocumentLink, FSharpOption<string>, string> resolver)
		{
			FSharpFunc<Fragments.DocumentLink, FSharpFunc<FSharpOption<string>, string>> f = resolver.FCurry ();
			return prismic.Api.DocumentLinkResolver.For (api, f);
		}			

		private static FSharpFunc<A, FSharpFunc<B, R>> FCurry<A, B, R>(this Func<A, B, R> f)
		{
			return
				CSharpAdapters.CreateFunc<A, FSharpFunc<B, R>> (a => CSharpAdapters.CreateFunc<B, R>(b => f (a, b)));
		}
	}


	public static class FragmentsExtensions
	{
		public static FSharpOption<Fragments.Text> GetTitle(this Fragments.Fragment fragment)
		{
			return FragmentsGetters.getTitle (fragment);
		}

		public static FSharpOption<Fragments.Text> GetFirstParagraph(this Fragments.Fragment fragment)
		{
			return FragmentsGetters.getFirstParagraph (fragment);
		}

		public static IEnumerable<Fragments.Text> GetAllParagraphs(this Fragments.Fragment fragment)
		{
			return FragmentsGetters.getAllParagraphs (fragment);
		}

		public static FSharpOption<Fragments.Block> GetFirstImage(this Fragments.Fragment fragment)
		{
			return FragmentsGetters.getFirstImage (fragment);
		}

		public static FSharpOption<Fragments.Fragment> Get(this Fragments.GroupDoc document, string field)
		{
			return document.fragments.Get (field);
		}
		public static IEnumerable<Fragments.Fragment> GetAll(this Fragments.GroupDoc document, string field)
		{
			return document.fragments.GetAll (field);
		}

		public static FSharpOption<Fragments.Fragment.Link> GetLink(this Fragments.GroupDoc document, string field)
		{
			return document.fragments.GetLink (field);
		}

		public static FSharpOption<Fragments.Fragment.Image> GetImage(this Fragments.GroupDoc document, string field)
		{
			return document.fragments.GetImage (field);
		}

		public static IEnumerable<Fragments.Fragment.Image> GetAllImages(this Fragments.GroupDoc document, string field)
		{
			return document.fragments.GetAllImages (field);
		}

		public static FSharpOption<Fragments.ImageView> GetImageView(this Fragments.GroupDoc document, string view, string field)
		{
			return document.fragments.GetImageView (view, field);
		}

		public static IEnumerable<Fragments.ImageView> GetAllImageViews(this Fragments.GroupDoc document, string view, string field)
		{
			return document.fragments.GetAllImageViews (view, field);
		}

		public static FSharpOption<Fragments.Fragment.StructuredText> GetStructuredText(this Fragments.GroupDoc document, string field)
		{
			return document.fragments.GetStructuredText (field);
		}

		public static FSharpOption<string> GetText(this Fragments.GroupDoc document, string field)
		{
			return document.fragments.GetText (field);
		}

		public static FSharpOption<Fragments.Fragment.Color> GetColor(this Fragments.GroupDoc document, string field)
		{
			return document.fragments.GetColor (field);
		}

		public static FSharpOption<Fragments.Fragment.Number> GetNumber(this Fragments.GroupDoc document, string field)
		{
			return document.fragments.GetNumber (field); 
		}

		public static FSharpOption<Fragments.Fragment.Date> GetDate(this Fragments.GroupDoc document, string field)
		{
			return document.fragments.GetDate (field); 
		}


		public static bool GetBoolean(this Fragments.GroupDoc document, string field)
		{
			return document.fragments.GetBoolean (field);
		}


		public static FSharpOption<Fragments.Fragment.Group> GetGroup(this Fragments.GroupDoc document, string field)
		{
			return document.fragments.GetGroup (field);
		}


		public static FSharpOption<Fragments.Fragment> Get(this prismic.Api.Document document, string field)
		{
			return document.fragments.Get (field);
		}

		public static IEnumerable<Fragments.Fragment> GetAll(this prismic.Api.Document document, string field)
		{
			return document.fragments.GetAll (field);
		}
			
		public static FSharpOption<Fragments.Fragment.Link> GetLink(this prismic.Api.Document document, string field)
		{
			return document.fragments.GetLink (field);
		}

		public static FSharpOption<Fragments.Fragment.Image> GetImage(this prismic.Api.Document document, string field)
		{
			return document.fragments.GetImage (field);
		}

		public static IEnumerable<Fragments.Fragment.Image> GetAllImages(this prismic.Api.Document document, string field)
		{
			return document.fragments.GetAllImages (field);
		}
			
		public static FSharpOption<Fragments.ImageView> GetImageView(this prismic.Api.Document document, string view, string field)
		{
			return document.fragments.GetImageView (view, field);
		}

		public static IEnumerable<Fragments.ImageView> GetAllImageViews(this prismic.Api.Document document, string view, string field)
		{
			return document.fragments.GetAllImageViews (view, field);
		}

		public static FSharpOption<Fragments.Fragment.StructuredText> GetStructuredText(this prismic.Api.Document document, string field)
		{
			return document.fragments.GetStructuredText (field);
		}

		public static FSharpOption<string> GetText(this prismic.Api.Document document, string field)
		{
			return document.fragments.GetText (field);
		}

		public static FSharpOption<Fragments.Fragment.Color> GetColor(this prismic.Api.Document document, string field)
		{
			return document.fragments.GetColor (field);
		}

		public static FSharpOption<Fragments.Fragment.Number> GetNumber(this prismic.Api.Document document, string field)
		{
			return document.fragments.GetNumber (field); 
		}

		public static FSharpOption<Fragments.Fragment.Date> GetDate(this prismic.Api.Document document, string field)
		{
			return document.fragments.GetDate (field); 
		}

		public static bool GetBoolean(this prismic.Api.Document document, string field)
		{
			return document.fragments.GetBoolean (field);
		}


		public static FSharpOption<Fragments.Fragment.Group> GetGroup(this prismic.Api.Document document, string field)
		{
			return document.fragments.GetGroup (field);
		}


		public static FSharpOption<Fragments.Fragment> Get(this prismic.TupleList<string, Fragments.Fragment> fragmentsMap, string field)
		{
			return FragmentsGetters.get(field, fragmentsMap);
		}

		public static IEnumerable<Fragments.Fragment> GetAll(this prismic.TupleList<string, Fragments.Fragment> fragmentsMap, string field)
		{
			return FragmentsGetters.getAll(field, fragmentsMap);
		}


		private static FSharpOption<Fragments.Fragment.Link> GetLink(this prismic.TupleList<string, Fragments.Fragment> fragmentsMap, string field)
		{
			var linkOption = FragmentsGetters.getLink (field, fragmentsMap);
			var map = CSharpAdapters.CreateFunc<Fragments.Fragment, Fragments.Fragment.Link>(o => (Fragments.Fragment.Link)o);
			return OptionModule.Map(map, linkOption);
		}

		private static FSharpOption<Fragments.Fragment.Image> GetImage(this prismic.TupleList<string, Fragments.Fragment> fragmentsMap, string field)
		{
			var imageOption = FragmentsGetters.getImage (field, fragmentsMap);
			var map = CSharpAdapters.CreateFunc<Fragments.Fragment, Fragments.Fragment.Image>(o => (Fragments.Fragment.Image)o);
			return OptionModule.Map(map, imageOption);
		}

		private static IEnumerable<Fragments.Fragment.Image> GetAllImages(this prismic.TupleList<string, Fragments.Fragment> fragmentsMap, string field)
		{
			var images = FragmentsGetters.getAllImages (field, fragmentsMap);
			return images.Cast<Fragments.Fragment.Image>();
		}

		private static FSharpOption<Fragments.ImageView> GetImageView(this prismic.TupleList<string, Fragments.Fragment> fragmentsMap, string view, string field)
		{
			var imageView = FragmentsGetters.getImageView (view, field, fragmentsMap);
			return imageView;
		}

		private static IEnumerable<Fragments.ImageView> GetAllImageViews(this prismic.TupleList<string, Fragments.Fragment> fragmentsMap, string view, string field)
		{
			var views = FragmentsGetters.getAllImageViews (field, field, fragmentsMap);
			return views;
		}

		private static FSharpOption<Fragments.Fragment.StructuredText> GetStructuredText(this prismic.TupleList<string, Fragments.Fragment> fragmentsMap, string field)
		{
			var textOption = FragmentsGetters.getStructuredText (field, fragmentsMap);
			var map = CSharpAdapters.CreateFunc<Fragments.Fragment, Fragments.Fragment.StructuredText>(o => (Fragments.Fragment.StructuredText)o);
			return OptionModule.Map(map, textOption);
		}

		private static FSharpOption<string> GetText(this prismic.TupleList<string, Fragments.Fragment> fragmentsMap, string field)
		{
			return FragmentsGetters.getText (field, fragmentsMap);
		}

		private static FSharpOption<Fragments.Fragment.Color> GetColor(this prismic.TupleList<string, Fragments.Fragment> fragmentsMap, string field)
		{
			var colorOption = FragmentsGetters.getColor (field, fragmentsMap);
			var map = CSharpAdapters.CreateFunc<Fragments.Fragment, Fragments.Fragment.Color>(o => (Fragments.Fragment.Color)o);
			return OptionModule.Map(map, colorOption);
		}

		private static FSharpOption<Fragments.Fragment.Number> GetNumber(this prismic.TupleList<string, Fragments.Fragment> fragmentsMap, string field)
		{
			var numberOption = FragmentsGetters.getNumber (field, fragmentsMap);
			var map = CSharpAdapters.CreateFunc<Fragments.Fragment, Fragments.Fragment.Number>(o => (Fragments.Fragment.Number)o);
			return OptionModule.Map(map, numberOption);
		}

		private static FSharpOption<Fragments.Fragment.Date> GetDate(this prismic.TupleList<string, Fragments.Fragment> fragmentsMap, string field)
		{
			var dateOption = FragmentsGetters.getDate (field, fragmentsMap);
			var map = CSharpAdapters.CreateFunc<Fragments.Fragment, Fragments.Fragment.Date>(o => (Fragments.Fragment.Date)o);
			return OptionModule.Map(map, dateOption);
		}


		private static bool GetBoolean(this prismic.TupleList<string, Fragments.Fragment> fragmentsMap, string field)
		{
			return FragmentsGetters.getBoolean (field, fragmentsMap);
		}


		private static FSharpOption<Fragments.Fragment.Group> GetGroup(this prismic.TupleList<string, Fragments.Fragment> fragmentsMap, string field)
		{
			var groupOption = FragmentsGetters.getGroup (field, fragmentsMap);
			var map = CSharpAdapters.CreateFunc<Fragments.Fragment, Fragments.Fragment.Group>(o => (Fragments.Fragment.Group)o);
			return OptionModule.Map(map, groupOption);
		}

		/// <summary>
		/// Extracts a media link, if there is some, from a link, if there is some.
		/// </summary>
		/// <returns>Maybe the media link.</returns>
		/// <param name="link">Maybe a Link.</param>
		public static FSharpOption<Fragments.MediaLink> BindAsMediaLink(this FSharpOption<Fragments.Fragment.Link> link)
		{
			var map = CSharpAdapters.CreateFunc<Fragments.Fragment.Link, FSharpOption<Fragments.MediaLink>> (
				l => l.Item.IsMediaLink 
				? FSharpOption<Fragments.MediaLink>.Some(((Fragments.Link.MediaLink)l.Item).Item) 
				: FSharpOption<Fragments.MediaLink>.None);
			return OptionModule.Bind (map, link);
		}
		/// <summary>
		/// Extracts a document link, if there is some, from a link, if there is some.
		/// </summary>
		/// <returns>Maybe the document link.</returns>
		/// <param name="link">Maybe a Link.</param>
		public static FSharpOption<Fragments.DocumentLink> BindAsDocumentLink(this FSharpOption<Fragments.Fragment.Link> link)
		{
			var map = CSharpAdapters.CreateFunc<Fragments.Fragment.Link, FSharpOption<Fragments.DocumentLink>> (
				l => l.Item.IsDocumentLink 
				? FSharpOption<Fragments.DocumentLink>.Some(((Fragments.Link.DocumentLink)l.Item).Item) 
				: FSharpOption<Fragments.DocumentLink>.None);
			return OptionModule.Bind (map, link);
		}
		/// <summary>
		/// Extracts a web link, if there is some, from a link, if there is some.
		/// </summary>
		/// <returns>Maybe the web link.</returns>
		/// <param name="link">Maybe a Link.</param>
		public static FSharpOption<Fragments.WebLink> BindAsWebLink(this FSharpOption<Fragments.Fragment.Link> link)
		{
			var map = CSharpAdapters.CreateFunc<Fragments.Fragment.Link, FSharpOption<Fragments.WebLink>> (
				l => l.Item.IsWebLink
				? FSharpOption<Fragments.WebLink>.Some(((Fragments.Link.WebLink)l.Item).Item) 
				: FSharpOption<Fragments.WebLink>.None);
			return OptionModule.Bind (map, link);
		}
			
		/// <summary>
		/// Extracts a media link, if there is some, from a fragment, if there is some.
		/// </summary>
		/// <returns>Maybe the media link.</returns>
		/// <param name="link">Maybe a Link.</param>
		public static FSharpOption<Fragments.MediaLink> BindAsMediaLink(this FSharpOption<Fragments.Fragment> fragment)
		{
			var map = CSharpAdapters.CreateFunc<Fragments.Fragment, FSharpOption<Fragments.Fragment.Link>> (
				l => l.IsLink
				? FSharpOption<Fragments.Fragment.Link>.Some((Fragments.Fragment.Link)l) 
				: FSharpOption<Fragments.Fragment.Link>.None);
			return OptionModule.Bind (map, fragment).BindAsMediaLink();
		}
		/// <summary>
		/// Extracts a document link, if there is some, from a fragment, if there is some.
		/// </summary>
		/// <returns>Maybe the document link.</returns>
		/// <param name="link">Maybe a Link.</param>
		public static FSharpOption<Fragments.DocumentLink> BindAsDocumentLink(this FSharpOption<Fragments.Fragment> fragment)
		{
			var map = CSharpAdapters.CreateFunc<Fragments.Fragment, FSharpOption<Fragments.Fragment.Link>> (
				l => l.IsLink
				? FSharpOption<Fragments.Fragment.Link>.Some((Fragments.Fragment.Link)l) 
				: FSharpOption<Fragments.Fragment.Link>.None);
			return OptionModule.Bind (map, fragment).BindAsDocumentLink();
		}
		/// <summary>
		/// Extracts a web link, if there is some, from a fragment, if there is some.
		/// </summary>
		/// <returns>Maybe the web link.</returns>
		/// <param name="link">Maybe a Link.</param>
		public static FSharpOption<Fragments.WebLink> BindAsWebLink(this FSharpOption<Fragments.Fragment> fragment)
		{
			var map = CSharpAdapters.CreateFunc<Fragments.Fragment, FSharpOption<Fragments.Fragment.Link>> (
				l => l.IsLink
				? FSharpOption<Fragments.Fragment.Link>.Some((Fragments.Fragment.Link)l) 
				: FSharpOption<Fragments.Fragment.Link>.None);
			return OptionModule.Bind (map, fragment).BindAsWebLink();
		}

		/// <summary>
		/// Tries to return HTML from a fragment option, given the link resolver.
		/// </summary>
		/// <returns>Maybe the HTML.</returns>
		/// <param name="fragment">the fragment option.</param>
		/// <param name="linkResolver">link Resolver.</param>
		public static FSharpOption<string> BindAsHtml<T>(this FSharpOption<T> fragment, prismic.Api.DocumentLinkResolver linkResolver)
			where T : Fragments.Fragment
		{
			var applyResolver = CSharpAdapters.CreateFunc<Fragments.DocumentLink, string> (l => linkResolver.Apply (l));
			var map = CSharpAdapters.CreateFunc<T, string> (f => FragmentsHtml.asHtml (applyResolver, f));
			return OptionModule.Map(map, fragment);
		}

		/// <summary>
		/// Tries to return HTML from a fragment, given the link resolver.
		/// </summary>
		/// <returns>The HTML.</returns>
		/// <param name="fragment">the fragment.</param>
		/// <param name="linkResolver">link Resolver.</param>
		public static string AsHtml<T>(this T fragment, prismic.Api.DocumentLinkResolver linkResolver)
			where T : Fragments.Fragment
		{
			var applyResolver = CSharpAdapters.CreateFunc<Fragments.DocumentLink, string> (l => linkResolver.Apply (l));
			return FragmentsHtml.asHtml (applyResolver, fragment);
		}
		/*
		public static FSharpOption<string> BindAsHtml(this FSharpOption<Fragments.Fragment> fragment, prismic.Api.DocumentLinkResolver linkResolver)
		{
			var applyResolver = CSharpAdapters.CreateFunc<Fragments.DocumentLink, string> (l => linkResolver.Apply (l));
			var map = CSharpAdapters.CreateFunc<Fragments.Fragment, string> (f => FragmentsHtml.asHtml (applyResolver, f));
			return OptionModule.Map(map, fragment);
		}*/

		/// <summary>
		/// Tries to return HTML from an image view option
		/// </summary>
		/// <returns>The html.</returns>
		/// <param name="imageView">Image view.</param>
		public static FSharpOption<string> BindAsHtml(this FSharpOption<Fragments.ImageView> imageView)
		{
			var map = CSharpAdapters.CreateFunc<Fragments.ImageView, string> (i => FragmentsHtml.imageViewAsHtml (i));
			return OptionModule.Map(map, imageView);
		}


		/// <summary>
		/// Tries to return HTML from a document, given the link resolver.
		/// </summary>
		/// <returns>The HTML.</returns>
		/// <param name="fragment">the document.</param>
		/// <param name="linkResolver">link Resolver.</param>
		public static string AsHtml(this prismic.Api.Document document, prismic.Api.DocumentLinkResolver linkResolver)
		{
			return prismic.Api.documentAsHtml (linkResolver, document);
		}
	}


}

