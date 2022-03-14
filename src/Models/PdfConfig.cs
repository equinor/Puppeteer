using Newtonsoft.Json;

namespace madpdf.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class PdfConfig
    {
        [JsonProperty("format")]
        public string format { get; set; }
        [JsonProperty("landscape")]
        public bool landscape { get; set; }
        [JsonProperty("margin", IsReference = true, Order = 999)]
        public PdfMargin margin { get; set; }
        [JsonProperty("preferCSSPageSize")]
        public bool preferCSSPageSize { get; set; }
        [JsonProperty("width")]
        public string width { get; set; }
        [JsonProperty("height")]
        public string height { get; set; }
        [JsonProperty("printBackground")]
        public bool printBackground { get; set; }
        [JsonProperty("scale")]
        public double scale { get; set; }
        [JsonProperty("displayHeaderFooter")]
        public bool displayHeaderFooter { get; set; }
        [JsonProperty("headerTemplate")]
        public string headerTemplate { get; set; }
        [JsonProperty("footerTemplate")]
        public string footerTemplate { get; set; }
    }
}