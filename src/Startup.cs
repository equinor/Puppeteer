using System;
using madpdf.Models;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using Hellang.Middleware.ProblemDetails;
using Statoil.MadCommon;
using Statoil.MadCommon.Authentication;
using Statoil.MadCommon;
using Statoil.MadCommon.Authentication;
using Statoil.MadCommon;
using Statoil.MadCommon.Authentication;
using Statoil.MadCommon;
using Statoil.MadCommon.Authentication;
using Statoil.MadCommon;
using Statoil.MadCommon.Authentication;


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

            services.AddProblemDetails(o =>
            {
                o.IncludeExceptionDetails = (ctx, ex) => true;
            });

            // Configure options
            services.AddOptions();
            services.AddRouting(options => options.LowercaseUrls = true);

            // Configure logging
            services.AddLogging(builder => builder
                .AddConsole()
#if DEBUG
                .AddFilter(level => level >= LogLevel.Debug));
#else
                .AddFilter(level => level >= LogLevel.Warning));
            // Add Swagger
            Bootstrap.AddSwagger(services, "Equinor PDF API", GetSwaggerDocPath());
        }

        private string GetSwaggerDocPath()
        {
            // Set the comments path for the Swagger JSON and UI.
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            app.UseProblemDetails();

            return xmlPath;
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory, TelemetryClient telemetryClient, IServiceProvider serviceProvider, IApiVersionDescriptionProvider provider)
        {
            app.UseResponseCompression();
            MapsterConfiguration.Configure();
            Bootstrap.AddDefaultExceptionHandling(app, telemetryClient);

            app.UseSwaggerUI();

            if (env.EnvironmentName == "dev")
                app.UseCors(builder =>
                    builder
                        .WithOrigins("http://localhost:5000", "http://localhost:5001")
                        .AllowAnyHeader()
                        .AllowAnyMethod());
            if (Convert.ToBoolean(Configuration["CertificateValidation:Enabled"]))
                app.UseClientCertificateAuthentication();

            app.UseMvc();
        }
    }
}