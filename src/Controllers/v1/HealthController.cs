using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.NodeServices;
using Statoil.MadCommon.Model.HealthCheck;


namespace madpdf.Controllers.v1
{
    [ApiVersion("1.0")]
    [Route("api/v{api-version:apiVersion}/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {

        private readonly INodeServices _nodeServices;


        public HealthController(INodeServices nodeServices)
        {
            _nodeServices = nodeServices;
        }

        /// <summary>
        /// Get health status for this api
        /// </summary>
        /// <returns></returns>
        [Produces(typeof(Health))]
        [HttpGet(Name = "GetHealth")]
        public IActionResult GetHealth()
        {
            Health health = new Health();
            var depencency = AzureAuthenticationHealthCheck();
            health.AddDepencency(depencency);
            if (depencency.Ok)
            {
                health.AddDepencency(Html2PdfHealthcheck().Result);
            }

            return Ok(health);
        }

        private Depencency AzureAuthenticationHealthCheck()
        {
            Depencency depencency = new Depencency("Azure authentication", "Get username from User.identity.Name");
            bool authenticated = User.Identity.IsAuthenticated;

            if (!authenticated)
            {
                depencency.SetError("No user");
            }

            return depencency;
        }

        private async Task<Depencency> Html2PdfHealthcheck()
        {
            Depencency depencency = new Depencency("Html2Pdf", "Detect if pdf is generated");

            try
            {
                var html = "<p>test/<p>";
                var response = await _nodeServices.InvokeAsync<string>("./Node/htmlToPdf.js", html);

                if (response == null)
                {
                    depencency.SetError("No response from Node");
                }
                else if (response.Length == 0)
                {
                    depencency.SetError("PDF stream is zero length");
                }
                else
                {
                    System.IO.File.Delete(response);
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
