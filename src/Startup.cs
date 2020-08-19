using System;
using System.IO;
using System.Reflection;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Statoil.MadCommon;
using Statoil.MadCommon.Authentication;

namespace Puppeteer
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
            // Configure options
            services.AddOptions();

            services.AddControllers();
            services.AddApiVersioning();
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
            
            services.AddNodeServices(options =>
            {
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
            Bootstrap.AddSwagger(services, "Equinor PDF API", GetSwaggerDocPath());
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
            app.UseRouting();
            app.UseApiVersioning();
            app.UseStaticFiles();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseResponseCompression();
            Bootstrap.AddDefaultExceptionHandling(app, telemetryClient);

            app.UseAuthentication();

            if (env.EnvironmentName == "dev")
                app.UseCors(builder =>
                    builder
                        .WithOrigins("http://localhost:5001")
                        .AllowAnyHeader()
                        .AllowAnyMethod());
            if (Convert.ToBoolean(Configuration["CertificateValidation:Enabled"]))
                app.UseClientCertificateAuthentication();

            app.UseMvc();
            Bootstrap.UseSwagger(app, provider, env);
        }
    }
}