using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using prismic;
using prismic.fragments;

namespace prismicio.AspNetCore.MvcSample
{
    public class DefaultDocumentLinkResolver : DocumentLinkResolver
    {
        private readonly LinkGenerator _linkGenerator;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DefaultDocumentLinkResolver(LinkGenerator linkGenerator, IHttpContextAccessor httpContextAccessor)
        {
            _linkGenerator = linkGenerator;
            _httpContextAccessor = httpContextAccessor;
        }
        public override string Resolve(DocumentLink link)
        {
            return _linkGenerator.GetPathByAction(_httpContextAccessor.HttpContext, nameof(Controllers.MvcSample.Index), nameof(Controllers.MvcSample));
        }
    }
}
