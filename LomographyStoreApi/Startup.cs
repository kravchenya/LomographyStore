using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using LomographyStoreApi.Services;
using LomographyStoreApi.Services.Interfaces;

namespace LomographyStoreApi
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
            IConfiguration dbConfig = Configuration.GetSection(Constants.KEY_DB_CONFIG);
            services.Configure<CosmosDBServiceOptions>(dbConfig);

            //doc client to communicate with cosmosdb 
            var docClient = new DocumentClient(new Uri(Configuration[Constants.KEY_COSMOS_URI]), Configuration[Constants.KEY_COSMOS_KEY]);
            services.AddSingleton<DocumentClient>(docClient);

            //Add storage service implementations
            services.AddScoped<IQueueService, AzureQueueService>();
            services.AddScoped<ITableService, AzureTableService>();
            services.AddScoped<IDocumentDBService, CosmosDBService>();
            services.AddScoped<IBlobService, AzureBlobService>();
            
            services.AddControllers();

            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new OpenApiInfo{
                    Title = "LomographyStoreAPI",
                    Version = "1.0",
                    Description = " Product API for LomographyStore"
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();

            app.UseSwaggerUI(c => {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Lomo Store");
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
