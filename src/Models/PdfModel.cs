using System.ComponentModel.DataAnnotations;

namespace madpdf.Models
{
    public class PdfModel
    {
        [Required] public string html { get; set; }

        // https://github.com/GoogleChrome/puppeteer/blob/master/docs/api.md#pagepdfoptions
        public PdfConfig config { get; set; }
    }
}