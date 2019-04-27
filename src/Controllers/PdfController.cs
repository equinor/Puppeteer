using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.NodeServices;
using Puppeteer.Models;
using Microsoft.AspNetCore.Http;
using System.Reflection;

namespace Puppeteer.Controllers
{
    [Route("api/v1/[controller]")]
    public class PdfController : Controller
    {
        private readonly INodeServices _nodeServices;

  
        public PdfController(INodeServices nodeServices)
        {
            _nodeServices = nodeServices;
        }

        [HttpPost]
        [Route("Html2Pdf")]
        public async Task<IActionResult> Html2Pdf([FromBody] PdfModel model)
        {
            var stream = await _nodeServices.InvokeAsync<Stream>("./Node/htmlToPdf.js", model.html);

            return File(stream, "application/pdf");
        }


        [HttpPost]
        [Route("Pdf2Png")]
        public async Task<IActionResult> Pdf2Png([FromForm] IFormCollection form, int page = 0, int dpi = 180)
        {
       
            var pdf = form.Files.FirstOrDefault();

            using (var ms = new MemoryStream())
            {
                pdf.CopyTo(ms);

                string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"tmp/");
                if (!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path);
                }

                var file = path + "test.pdf";

                System.IO.File.WriteAllBytes(file, ms.ToArray());

                var img = await _nodeServices.InvokeAsync<string>("./Node/pdfToImage.js", file, page, dpi);

                var bytes = System.IO.File.ReadAllBytes(img);
                return File(bytes, "image/png");

            }



        }
    }
}