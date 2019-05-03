using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Statoil.MadCommon;
using Statoil.MadCommon.Authentication;
using Statoil.MadCommon.Swagger;
using Swashbuckle.AspNetCore.Swagger;
using System;

namespace Puppeteer
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940

        public IConfiguration Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }


        public void ConfigureServices(IServiceCollection services)
        {



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

            services.AddMvc();
            services.AddApiVersioning(o => o.ReportApiVersions = true);
            services.AddMvcCore();
            services.AddVersionedApiExplorer(o => o.GroupNameFormat = "'v'VVV");

            services.AddNodeServices(options => {
                options.InvocationTimeoutMilliseconds = 300000;
                options.LaunchWithDebugging = true;
                options.DebuggingPort = 9229;
            });



            // Configure authentication
            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
                .AddCookie()
                .AddJwtBearer(o =>
                {
                    o.Authority = Configuration["AzureAd:Authority"];
                    o.Audience = Configuration["AzureAd:ClientId"];
#if DEBUG
                    o.RequireHttpsMetadata = false;
#endif
                });

            // Add Swagger
            AddSwagger(services);


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, TelemetryClient telemetryClient, IServiceProvider serviceProvider, IApiVersionDescriptionProvider provider)
        {
            loggerFactory.AddApplicationInsights(serviceProvider);
            app.UseResponseCompression();
            Bootstrap.AddDefaultExceptionHandling(app, telemetryClient);

            app.UseAuthentication();

            if (env.EnvironmentName == "dev")
            {
                app.UseCors(builder =>
                    builder
                        .WithOrigins("http://localhost:5001")
                        .AllowAnyHeader()
                        .AllowAnyMethod());
            }
            if (Convert.ToBoolean(Configuration["CertificateValidation:Enabled"]))
            {
                app.UseClientCertificateAuthentication();
            }

            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                // build a swagger endpoint for each discovered API version
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                }
            });
        }


        private void AddSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(o =>
            {
                var provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    o.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
                }
                o.OperationFilter<ImplicitApiVersionParameter>();
            });
        }

        private Info CreateInfoForApiVersion(ApiVersionDescription description)
        {
            var info = new Info()
            {
                Title = $"Equinor PDF API {description.ApiVersion}",
                Version = description.ApiVersion.ToString(),
                Description = "API for converting html to pdf and pdf to png",
            };

            if (description.IsDeprecated)
            {
                info.Description += " This API version has been deprecated.";
            }

            return info;
        }
    }
}
