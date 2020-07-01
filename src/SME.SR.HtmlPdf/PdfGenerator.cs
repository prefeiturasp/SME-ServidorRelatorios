using DinkToPdf;
using DinkToPdf.Contracts;
using Sentry;
using System;
using System.Collections.Generic;
using System.IO;

namespace SME.SR.HtmlPdf
{
    public class PdfGenerator : IReportConverter
    {

        public readonly IConverter converter;

        public PdfGenerator(IConverter converter)
        {
            this.converter = converter;
        }

        public void ConvertToPdf(List<string> paginas, string nomeArquivo)
        {
            ConvertToPdf(paginas, null, nomeArquivo);
        }

        public byte[] ConvertToPdf(List<string> paginas)
        {
            HtmlToPdfDocument doc = StartBasicDoc(paginas);

            byte[] pdf = converter.Convert(doc);

            return pdf;
        }

        public void ConvertToPdf(List<string> paginas, string caminhoBase, string nomeArquivo)
        {
            HtmlToPdfDocument doc = StartBasicDoc(paginas);

            if (!string.IsNullOrWhiteSpace(nomeArquivo))
            {
                nomeArquivo = String.Format("{0}.pdf", nomeArquivo);

                if (!string.IsNullOrWhiteSpace(caminhoBase))
                {
                    SentrySdk.AddBreadcrumb($"Caminho arquivo de relatório: {Path.Combine(caminhoBase, $"relatorios", nomeArquivo)}");
                    doc.GlobalSettings.Out = Path.Combine(caminhoBase, $"relatorios", nomeArquivo);
                }
                else
                {
                    doc.GlobalSettings.Out = nomeArquivo;
                }
            }

            converter.Convert(doc);
        }

        private static HtmlToPdfDocument StartBasicDoc(List<string> paginas)
        {
            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Landscape,
                    PaperSize = PaperKind.A4,
                    Margins = new MarginSettings() { Top = 5, Bottom = 5, Left = 5, Right = 5 }
                }
            };


            foreach (var pagina in paginas)
            {
                doc.Objects.Add(new ObjectSettings()
                {
                    HtmlContent = pagina,
                    WebSettings = { DefaultEncoding = "utf-8" }
                });
            }

            return doc;
        }
    }
}
