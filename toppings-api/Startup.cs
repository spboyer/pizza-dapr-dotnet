using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapr.Extensions.Configuration;
using Dapr.Client;

namespace toppings_api
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            HostingEnvironment = env;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment HostingEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddSingleton<ITelemetryInitializer, CloudRoleNameInitializer>();

            // Database Exception Page
            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddControllers().AddDapr();


            var daprClient = new DaprClientBuilder().Build();
            var settings = daprClient.GetBulkSecretAsync("local-secret-store").Result;
            var connection = settings["connectionStrings:mongo"].Values.First();


            //// Health Check
            //// https://docs.microsoft.com/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-5.0
            services.AddHealthChecks()
            .AddMongoDb(mongodbConnectionString: connection, //Configuration.GetConnectionString("mongo"),
                name: "mongo",
                failureStatus: HealthStatus.Unhealthy); //adding MongoDb Health Check


            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "toppings_api", Version = "v1" });
            });

            services.AddTransient<IApplicationDbContext, ToppingsDbContext>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "toppings_api v1"));
            }

            //app.UseHttpsRedirection();

            app.UseRouting();

            //app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                //endpoints.MapHealthChecks("/health/startup", new HealthCheckOptions()
                //{
                //    AllowCachingResponses = false
                //});
                //endpoints.MapHealthChecks("/healthz");
                //endpoints.MapHealthChecks("/ready");
            });
        }

        private void CheckandSeed(IServiceProvider services)
        {
            using (var scope = services.CreateScope())
            {
                var db = scope.ServiceProvider.GetService<IApplicationDbContext>();

                if (db.Toppings.CountDocuments(new BsonDocument()) == 0)
                {
                    SeedData.Initialize(db);
                }
            }
        }
    }
}
