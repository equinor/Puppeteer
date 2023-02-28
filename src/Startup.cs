using System;
using System.IO;
using System.Reflection;
using madpdf.Models;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using Newtonsoft.Json;
using PuppeteerSharp;

namespace madpdf
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true);

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940

        public IConfiguration Configuration { get; }


        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMicrosoftIdentityWebApiAuthentication(this.Configuration, "AzureAd", "Bearer", false);
                
            // Configure options
            services.AddOptions();

            // Configure logging
            services.AddLogging(builder => builder
                .AddConsole()
#if DEBUG
                .AddFilter(level => level >= LogLevel.Debug));
#else
                .AddFilter(level => level >= LogLevel.Warning));
#endif
            // Needs ApplicationInsights__InstrumentationKey in appsettings to work
            services.AddApplicationInsightsTelemetry(Configuration);
            services.AddResponseCompression();
            services.AddMemoryCache();
            services.AddMvc(o => o.EnableEndpointRouting = false)
                .AddNewtonsoftJson();
            services.AddApiVersioning(o => o.ReportApiVersions = true);

            services.AddVersionedApiExplorer(o => o.GroupNameFormat = "'v'VVV");

            // Configure authentication
            

            // Add Swagger
            services.AddSwaggerGen();
        }

        private string GetSwaggerDocPath()
        {
            // Set the comments path for the Swagger JSON and UI.
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            return xmlPath;
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory,
            TelemetryClient telemetryClient, IServiceProvider serviceProvider, IApiVersionDescriptionProvider provider)
        {
            app.UseAuthentication();
            app.UseResponseCompression();
            MapsterConfiguration.Configure();

            app.UseRouting();
            app.UseSwagger();
            app.UseSwaggerUI();

            if (env.EnvironmentName == "dev")
                app.UseCors(builder =>
                    builder
                        .WithOrigins("http://localhost:5000", "http://localhost:5001")
                        .AllowAnyHeader()
                        .AllowAnyMethod());
            if (Convert.ToBoolean(Configuration["CertificateValidation:Enabled"]))

            app.UseMvc();
        }
    }
}