using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using prismic;

namespace prismicio.AspNetCore.MvcSample.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IPrismicApiAccessor prismic;

        public IndexModel(IPrismicApiAccessor prismic)
        {
            this.prismic = prismic;
        }
        public async Task OnGet()
        {
            var api = await prismic.GetApi();
            ViewData.Add("MasterRef", api.Master.Reference);
        }
    }
}
