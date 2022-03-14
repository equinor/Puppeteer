using Newtonsoft.Json;

namespace madpdf.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class PdfMargin
    {
        public PdfMargin()
        {
        }
        public PdfMargin(string top, string right, string bottom, string left)
        {
            this.top = top;
            this.right = right;
            this.bottom = bottom;
            this.left = left;
        }
        [JsonProperty("top")]
        public string top { get; set; }
        [JsonProperty("right")]
        public string right { get; set; }
        [JsonProperty("bottom")]
        public string bottom { get; set; }
        [JsonProperty("left")]
        public string left { get; set; }
    }
}