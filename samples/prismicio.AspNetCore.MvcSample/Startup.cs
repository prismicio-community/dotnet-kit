using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using prismic;

namespace prismicio.AspNetCore.MvcSample
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddPrismic();
            services.Configure<PrismicSettings>(Configuration.GetSection("prismic"));
            services.AddSingleton<DocumentLinkResolver, DefaultDocumentLinkResolver>();
            services.AddRazorPages();
            services.AddControllers().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            // Handle exceptions caused when preview tokens expire...
            app.UsePrismicPreviewExpiredMiddleware();
            app.UseRouting();

            app.UseEndpoints(s =>
            {
                s.MapRazorPages();
                s.MapDefaultControllerRoute();
            });
        }
    }
}
