using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using madpdf.Models;
using Mapster;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PuppeteerSharp;
using PuppeteerSharp.Media;

namespace madpdf.Controllers.v1
{
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{api-version:apiVersion}/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class PdfController : ControllerBase
    {
        private readonly TelemetryClient _telemetryClient;


        public PdfController(TelemetryClient telemetryClient)
        {
            _telemetryClient = telemetryClient;
        }

        [HttpPost]
        [Route("Html2Pdf", Name = "Html2Pdf")]
        public async Task<IActionResult> Html2Pdf([FromBody] PdfModel model)
        {
            try
            {
                var bfOptions = new BrowserFetcherOptions();
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    bfOptions.Path = Path.GetTempPath();
                }
                var bf = await new BrowserFetcher(bfOptions).DownloadAsync(BrowserFetcher.DefaultChromiumRevision);
                var options = new LaunchOptions { Headless = true,ExecutablePath = bf.ExecutablePath};
                using (var browser = await Puppeteer.LaunchAsync(options))
                {

                    if (model.config.scale == 0.0) model.config.scale = 1;

                    var payload = JsonConvert.SerializeObject(model.config,
                        Formatting.Indented,
                        new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore
                        });
                    var page = await browser.NewPageAsync();
                    //await page.GoToAsync("http://www.vg.no");
                    await page.SetContentAsync(model.html);
                    PdfOptions pdfOptions = new PdfOptions();
                    if (model.config != null)
                    {
                         pdfOptions = model.config.Adapt<PdfOptions>();
                        if (!string.IsNullOrEmpty(model.config.format))
                        {
                            switch (model.config.format.ToLower())
                            {
                                case "a0":
                                    pdfOptions.Format = PaperFormat.A0;
                                    break;
                                case "a1":
                                    pdfOptions.Format = PaperFormat.A1;
                                    break;
                                case "a2":
                                    pdfOptions.Format = PaperFormat.A2;
                                    break;
                                case "a3":
                                    pdfOptions.Format = PaperFormat.A3;
                                    break;
                                case "a5":
                                    pdfOptions.Format = PaperFormat.A5;
                                    break;
                                case "a6":
                                    pdfOptions.Format = PaperFormat.A6;
                                    break;
                                default:
                                    pdfOptions.Format = PaperFormat.A4;
                                    break;

                            }
                        }
                    }

                    //var pdfPath = await _nodeServices.InvokeFromFileAsync<string>("./Node/htmlToPdf.cjs", model.html, args:new object[]{payload});
                    var bytes = await page.PdfDataAsync(pdfOptions);
                    return File(bytes, "application/pdf");
                }
            }
            catch (Exception ex)
            {
                _telemetryClient.TrackException(ex);

                return BadRequest(ex.Message);
            }
        }

    }
}