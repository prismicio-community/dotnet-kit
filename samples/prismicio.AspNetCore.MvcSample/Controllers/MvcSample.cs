using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using prismic;

namespace prismicio.AspNetCore.MvcSample.Controllers
{
    public class MvcSample : Controller
    {
        private readonly IPrismicApiAccessor _prismic;
        private readonly DocumentLinkResolver _linkResolver;

        public MvcSample(IPrismicApiAccessor prismic, DocumentLinkResolver linkResolver)
        {
            _prismic = prismic;
            _linkResolver = linkResolver;
        }

        [Route("~/mvc")]
        public async Task<IActionResult> Index()
        {
            var api = await _prismic.GetApi();

            var docType = "test_document";

            var document = await api.GetByUID(docType, "test-document");

            return View(model: document.GetText($"{docType}.text"));
        }

        [Route("~/preview")]
        public async Task<IActionResult> Preview(string token)
        {
            try
            {
                var api = await _prismic.GetApi();
                var url = await api.PreviewSession(token, _linkResolver, "/");

                Response.Cookies.Append(Api.PREVIEW_COOKIE, token, new CookieOptions
                {
                    Expires = DateTime.Now.AddMinutes(30),
                });

                return Redirect(url);
            }
            catch (Exception)
            {
                return Redirect("/");
            }
        }

    }
}