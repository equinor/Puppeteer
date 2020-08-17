namespace madpdf.Models
{
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

        public string top { get; set; }
        public string right { get; set; }
        public string bottom { get; set; }
        public string left { get; set; }
    }
}