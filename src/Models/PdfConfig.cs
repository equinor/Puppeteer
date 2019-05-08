using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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


    }
}
