using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Jering.Javascript.NodeJS;
using madpdf.Models;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace madpdf.Controllers.v1
{
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{api-version:apiVersion}/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class PdfController : ControllerBase
    {
        private readonly INodeJSService _nodeServices;
        private readonly TelemetryClient _telemetryClient;


        public PdfController(INodeJSService nodeServices, TelemetryClient telemetryClient)
        {
            _nodeServices = nodeServices;
            _telemetryClient = telemetryClient;
        }

        [HttpPost]
        [Route("Html2Pdf", Name = "Html2Pdf")]
        public async Task<IActionResult> Html2Pdf([FromBody] PdfModel model)
        {
            try
            {
                if (model.config.scale == 0.0) model.config.scale = 1;

                var payload = JsonConvert.SerializeObject(model.config,
                    Formatting.Indented,
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });

                var pdfPath = await _nodeServices.InvokeFromFileAsync<string>("./Node/htmlToPdf.js", model.html, args:new object[]{payload});
                var bytes = System.IO.File.ReadAllBytes(pdfPath);
                return File(bytes, "application/pdf");
            }
            catch (Exception ex)
            {
                _telemetryClient.TrackException(ex);

                return BadRequest(ex.Message);
            }
        }


        [HttpPost]
        [Route("Pdf2Png", Name = "Pdf2Png")]
        public async Task<IActionResult> Pdf2Png([FromBody] IFormFileCollection form, int page = 0, int dpi = 180)
        {
            var pdf = form.FirstOrDefault();

            using (var ms = new MemoryStream())
            {
                pdf.CopyTo(ms);

                var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"tmp/");
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                var file = path + Guid.NewGuid().ToString("N") + ".pdf";

                System.IO.File.WriteAllBytes(file, ms.ToArray());

                var img = await _nodeServices.InvokeFromFileAsync<string>("./Node/pdfToImage.js", file, args: new object[]{page, dpi});

                var bytes = System.IO.File.ReadAllBytes(img);
                System.IO.File.Delete(img);
                return File(bytes, "image/png");
            }
        }
    }
}