using System;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using LomographyStoreWeb.Services;

namespace LomographyStoreWeb
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
            IConfiguration webapiConfig = Configuration.GetSection(Constants.KEY_WEB_API_ROUTES);
            services.Configure<WebApiRouteOptions>(webapiConfig);

            services.AddControllersWithViews();

            var apiClient = CreateHttpClient();
            services.AddSingleton<HttpClient>(apiClient);

            services.AddScoped<IHttCustomClient, HttCustomClient>();
        }

        private HttpClient CreateHttpClient()
        {
            Uri apiBaseUri = new Uri(Configuration[Constants.KEY_API_BASE_URI]);
            HttpClient apiClient = new HttpClient
            {
                BaseAddress = apiBaseUri
            };
            ServicePointManager.FindServicePoint(apiBaseUri).ConnectionLeaseTimeout = 60000;
            return apiClient;
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
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
