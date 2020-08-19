using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace madpdf.Models
{
    
    public class PdfModel
    {
        [JsonProperty("PdfModel")]
        [Required] 
        public string html { get; set; }

        // https://github.com/GoogleChrome/puppeteer/blob/master/docs/api.md#pagepdfoptions
        [JsonProperty("config")]
        public PdfConfig config { get; set; }
    }
}