using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mapster;
using PuppeteerSharp;

namespace madpdf.Models
{
    public class MapsterConfiguration
    {
        static MapsterConfiguration()
        {
            TypeAdapterConfig<PdfConfig, PdfOptions>.NewConfig()
                .Map(d => d.DisplayHeaderFooter, s => s.displayHeaderFooter)
                .Map(d => d.FooterTemplate, s => s.footerTemplate)
                .Map(d => d.HeaderTemplate, s => s.headerTemplate)
                .Map(d => d.Height, s => s.height)
                .Map(d => d.Landscape, s => s.landscape)
                .Map(d => d.MarginOptions, s => s.margin)
                .Map(d => d.PreferCSSPageSize, s => s.preferCSSPageSize)
                .Map(d => d.PrintBackground, s => s.printBackground)
                .Map(d => d.Scale, s => s.scale)
                .Map(d => d.Width, s => s.width)
                .Map(d => d.MarginOptions.Bottom, s => s.margin.bottom)
                .Map(d => d.MarginOptions.Left, s => s.margin.left)
                .Map(d => d.MarginOptions.Right, s => s.margin.right)
                .Map(d => d.MarginOptions.Top, s => s.margin.top);
        }

        public static void Configure() { }
    }
}
