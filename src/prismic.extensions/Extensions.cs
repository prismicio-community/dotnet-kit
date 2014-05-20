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
		public static Task<prismic.Api.Response> Submit(this prismic.Api.SearchForm form)
		{
			return FSharpAsync.StartAsTask (form.Submit(), FSharpOption<TaskCreationOptions>.None, FSharpOption<CancellationToken>.None);
		}

		public static Task<prismic.Api.Api> Get(string token, string url, prismic.Infra.ICache<prismic.Api.Response> cache, Action<string, string> logger)
		{
			return FSharpAsync.StartAsTask (prismic.Api.get(cache, logger.ToFSharpFunc(),
				FSharpOption<string>.Some(token), url), FSharpOption<TaskCreationOptions>.None , FSharpOption<CancellationToken>.None);
		}
		public static Task<prismic.Api.Api> Get(string url, prismic.Infra.ICache<prismic.Api.Response>  cache, Action<string, string> logger)
		{
			return FSharpAsync.StartAsTask (prismic.Api.get(cache, logger.ToFSharpFunc(),
				FSharpOption<string>.None, url), FSharpOption<TaskCreationOptions>.None , FSharpOption<CancellationToken>.None);
		}
		public static SearchFormWithTask SubmitableAsTask(this prismic.Api.SearchForm form)
		{
			return new SearchFormWithTask (form);
		}
	}
	public class SearchFormWithTask
	{
		private readonly prismic.Api.SearchForm form;
		internal SearchFormWithTask(prismic.Api.SearchForm form)
		{
			this.form = form;
		}
		public Task<prismic.Api.Response> Submit()
		{
			return FSharpAsync.StartAsTask (form.Submit(), FSharpOption<TaskCreationOptions>.None, FSharpOption<CancellationToken>.None);
		}
	}

	public static class DocumentLinkResolver
	{
		public static prismic.Api.DocumentLinkResolver For(System.Func<Fragments.DocumentLink, string> resolver)
		{
			return prismic.Api.DocumentLinkResolver.For(CoreEx.CreateFunc(resolver));
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


		public static FSharpOption<Fragments.Fragment> Get(this FSharpMap<string, Fragments.Fragment> fragmentsMap, string field)
		{
			return FragmentsGetters.get(field, fragmentsMap);
		}

		public static IEnumerable<Fragments.Fragment> GetAll(this FSharpMap<string, Fragments.Fragment> fragmentsMap, string field)
		{
			return FragmentsGetters.getAll(field, fragmentsMap);
		}


		private static FSharpOption<Fragments.Fragment.Link> GetLink(this FSharpMap<string, Fragments.Fragment> fragmentsMap, string field)
		{
			var linkOption = FragmentsGetters.getLink (field, fragmentsMap);
			var map = CoreEx.CreateFunc<Fragments.Fragment, Fragments.Fragment.Link>(o => (Fragments.Fragment.Link)o);
			return OptionModule.Map(map, linkOption);
		}

		private static FSharpOption<Fragments.Fragment.Image> GetImage(this FSharpMap<string, Fragments.Fragment> fragmentsMap, string field)
		{
			var imageOption = FragmentsGetters.getImage (field, fragmentsMap);
			var map = CoreEx.CreateFunc<Fragments.Fragment, Fragments.Fragment.Image>(o => (Fragments.Fragment.Image)o);
			return OptionModule.Map(map, imageOption);
		}

		private static IEnumerable<Fragments.Fragment.Image> GetAllImages(this FSharpMap<string, Fragments.Fragment> fragmentsMap, string field)
		{
			var images = FragmentsGetters.getAllImages (field, fragmentsMap);
			return images.Cast<Fragments.Fragment.Image>();
		}

		private static FSharpOption<Fragments.ImageView> GetImageView(this FSharpMap<string, Fragments.Fragment> fragmentsMap, string view, string field)
		{
			var imageView = FragmentsGetters.getImageView (view, field, fragmentsMap);
			return imageView;
		}

		private static IEnumerable<Fragments.ImageView> GetAllImageViews(this FSharpMap<string, Fragments.Fragment> fragmentsMap, string view, string field)
		{
			var views = FragmentsGetters.getAllImageViews (field, field, fragmentsMap);
			return views;
		}

		private static FSharpOption<Fragments.Fragment.StructuredText> GetStructuredText(this FSharpMap<string, Fragments.Fragment> fragmentsMap, string field)
		{
			var textOption = FragmentsGetters.getStructuredText (field, fragmentsMap);
			var map = CoreEx.CreateFunc<Fragments.Fragment, Fragments.Fragment.StructuredText>(o => (Fragments.Fragment.StructuredText)o);
			return OptionModule.Map(map, textOption);
		}

		private static FSharpOption<string> GetText(this FSharpMap<string, Fragments.Fragment> fragmentsMap, string field)
		{
			return FragmentsGetters.getText (field, fragmentsMap);
		}

		private static FSharpOption<Fragments.Fragment.Color> GetColor(this FSharpMap<string, Fragments.Fragment> fragmentsMap, string field)
		{
			var colorOption = FragmentsGetters.getColor (field, fragmentsMap);
			var map = CoreEx.CreateFunc<Fragments.Fragment, Fragments.Fragment.Color>(o => (Fragments.Fragment.Color)o);
			return OptionModule.Map(map, colorOption);
		}

		private static FSharpOption<Fragments.Fragment.Number> GetNumber(this FSharpMap<string, Fragments.Fragment> fragmentsMap, string field)
		{
			var numberOption = FragmentsGetters.getNumber (field, fragmentsMap);
			var map = CoreEx.CreateFunc<Fragments.Fragment, Fragments.Fragment.Number>(o => (Fragments.Fragment.Number)o);
			return OptionModule.Map(map, numberOption);
		}

		private static FSharpOption<Fragments.Fragment.Date> GetDate(this FSharpMap<string, Fragments.Fragment> fragmentsMap, string field)
		{
			var dateOption = FragmentsGetters.getDate (field, fragmentsMap);
			var map = CoreEx.CreateFunc<Fragments.Fragment, Fragments.Fragment.Date>(o => (Fragments.Fragment.Date)o);
			return OptionModule.Map(map, dateOption);
		}


		private static bool GetBoolean(this FSharpMap<string, Fragments.Fragment> fragmentsMap, string field)
		{
			return FragmentsGetters.getBoolean (field, fragmentsMap);
		}


		private static FSharpOption<Fragments.Fragment.Group> GetGroup(this FSharpMap<string, Fragments.Fragment> fragmentsMap, string field)
		{
			var groupOption = FragmentsGetters.getGroup (field, fragmentsMap);
			var map = CoreEx.CreateFunc<Fragments.Fragment, Fragments.Fragment.Group>(o => (Fragments.Fragment.Group)o);
			return OptionModule.Map(map, groupOption);
		}



		public static FSharpOption<Fragments.MediaLink> BindAsMediaLink(this FSharpOption<Fragments.Fragment.Link> link)
		{
			var map = CoreEx.CreateFunc<Fragments.Fragment.Link, FSharpOption<Fragments.MediaLink>> (
				l => l.Item.IsMediaLink 
				? FSharpOption<Fragments.MediaLink>.Some(((Fragments.Link.MediaLink)l.Item).Item) 
				: FSharpOption<Fragments.MediaLink>.None);
			return OptionModule.Bind (map, link);
		}
		public static FSharpOption<Fragments.DocumentLink> BindAsDocumentLink(this FSharpOption<Fragments.Fragment.Link> link)
		{
			var map = CoreEx.CreateFunc<Fragments.Fragment.Link, FSharpOption<Fragments.DocumentLink>> (
				l => l.Item.IsDocumentLink 
				? FSharpOption<Fragments.DocumentLink>.Some(((Fragments.Link.DocumentLink)l.Item).Item) 
				: FSharpOption<Fragments.DocumentLink>.None);
			return OptionModule.Bind (map, link);
		}
		public static FSharpOption<Fragments.WebLink> BindAsWebLink(this FSharpOption<Fragments.Fragment.Link> link)
		{
			var map = CoreEx.CreateFunc<Fragments.Fragment.Link, FSharpOption<Fragments.WebLink>> (
				l => l.Item.IsWebLink
				? FSharpOption<Fragments.WebLink>.Some(((Fragments.Link.WebLink)l.Item).Item) 
				: FSharpOption<Fragments.WebLink>.None);
			return OptionModule.Bind (map, link);
		}
			

		public static FSharpOption<Fragments.MediaLink> BindAsMediaLink(this FSharpOption<Fragments.Fragment> fragment)
		{
			var map = CoreEx.CreateFunc<Fragments.Fragment, FSharpOption<Fragments.Fragment.Link>> (
				l => l.IsLink
				? FSharpOption<Fragments.Fragment.Link>.Some((Fragments.Fragment.Link)l) 
				: FSharpOption<Fragments.Fragment.Link>.None);
			return OptionModule.Bind (map, fragment).BindAsMediaLink();
		}
		public static FSharpOption<Fragments.DocumentLink> BindAsDocumentLink(this FSharpOption<Fragments.Fragment> fragment)
		{
			var map = CoreEx.CreateFunc<Fragments.Fragment, FSharpOption<Fragments.Fragment.Link>> (
				l => l.IsLink
				? FSharpOption<Fragments.Fragment.Link>.Some((Fragments.Fragment.Link)l) 
				: FSharpOption<Fragments.Fragment.Link>.None);
			return OptionModule.Bind (map, fragment).BindAsDocumentLink();
		}
		public static FSharpOption<Fragments.WebLink> BindAsWebLink(this FSharpOption<Fragments.Fragment> fragment)
		{
			var map = CoreEx.CreateFunc<Fragments.Fragment, FSharpOption<Fragments.Fragment.Link>> (
				l => l.IsLink
				? FSharpOption<Fragments.Fragment.Link>.Some((Fragments.Fragment.Link)l) 
				: FSharpOption<Fragments.Fragment.Link>.None);
			return OptionModule.Bind (map, fragment).BindAsWebLink();
		}

		public static FSharpOption<string> BindAsHtml<T>(this FSharpOption<T> fragment, prismic.Api.DocumentLinkResolver linkResolver)
			where T : Fragments.Fragment
		{
			var applyResolver = CoreEx.CreateFunc<Fragments.DocumentLink, string> (l => linkResolver.Apply (l));
			var map = CoreEx.CreateFunc<T, string> (f => FragmentsHtml.asHtml (applyResolver, f));
			return OptionModule.Map(map, fragment);
		}
		/*
		public static FSharpOption<string> BindAsHtml(this FSharpOption<Fragments.Fragment> fragment, prismic.Api.DocumentLinkResolver linkResolver)
		{
			var applyResolver = CoreEx.CreateFunc<Fragments.DocumentLink, string> (l => linkResolver.Apply (l));
			var map = CoreEx.CreateFunc<Fragments.Fragment, string> (f => FragmentsHtml.asHtml (applyResolver, f));
			return OptionModule.Map(map, fragment);
		}*/
		public static FSharpOption<string> BindAsHtml(this FSharpOption<Fragments.ImageView> imageView)
		{
			var map = CoreEx.CreateFunc<Fragments.ImageView, string> (i => FragmentsHtml.imageViewAsHtml (i));
			return OptionModule.Map(map, imageView);
		}

	}


}

