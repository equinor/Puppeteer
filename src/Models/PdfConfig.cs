namespace madpdf.Models
{
    public class PdfConfig
    {
        public string format { get; set; }

        public bool landscape { get; set; }

        public PdfMargin margin { get; set; }

        public bool preferCSSPageSize { get; set; }
        public string width { get; set; }
        public string height { get; set; }

        public bool printBackground { get; set; }
        public double scale { get; set; }

        public bool displayHeaderFooter { get; set; }

        public string headerTemplate { get; set; }

        public string footerTemplate { get; set; }
    }
}