using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PuppeteerSharp;
using Statoil.MadCommon.Model.HealthCheck;

namespace madpdf.Controllers.v1
{
    [ApiVersion("1.0")]
    [Route("api/v{api-version:apiVersion}/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {



        public HealthController()
        {
        }

        /// <summary>
        ///     Get health status for this api
        /// </summary>
        /// <returns></returns>
        [Produces(typeof(Health))]
        [HttpGet(Name = "GetHealth")]
        public IActionResult GetHealth()
        {
            var health = new Health();
            var depencency = AzureAuthenticationHealthCheck();
            health.AddDepencency(depencency);
            if (depencency.Ok) health.AddDepencency(Html2PdfHealthcheck().Result);

            return Ok(health);
        }

        private Depencency AzureAuthenticationHealthCheck()
        {
            var depencency = new Depencency("Azure authentication", "Get username from User.identity.Name");
            var authenticated = User.Identity.IsAuthenticated;

            if (!authenticated) depencency.SetError("No user");

            return depencency;
        }

        private async Task<Depencency> Html2PdfHealthcheck()
        {
            var depencency = new Depencency("Html2Pdf", "Detect if pdf is generated");

            try
            {
                var bfOptions = new BrowserFetcherOptions();
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    bfOptions.Path = Path.GetTempPath();
                }

                var bf = await new BrowserFetcher(bfOptions).DownloadAsync(BrowserFetcher.DefaultChromiumRevision);
                var options = new LaunchOptions {Headless = true, ExecutablePath = bf.ExecutablePath, Args = new[] { "--no-sandbox" } };
                using (var browser = await Puppeteer.LaunchAsync(options))
                {
                    var html = "<p>test/<p>";
                    var page = await browser.NewPageAsync();
                    await page.SetContentAsync(html);
                    var response = await page.PdfStreamAsync();
                    //var response = await _nodeServices.InvokeFromFileAsync<string>("./Node/htmlToPdf.cjs", html);

                    if (response == null)
                        depencency.SetError("No response from Node");
                    else if (response.Length == 0)
                        depencency.SetError("PDF stream is zero length");
                }
            }
            catch (Exception e)
            {
                depencency.SetError(e);
            }

            return depencency;
        }
    }
}