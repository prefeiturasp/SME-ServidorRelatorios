using DinkToPdf;
using DinkToPdf.Contracts;
using Sentry;
using SME.SR.Infra.Dtos;
using System;
using System.Collections.Generic;
using System.IO;
using SME.SR.Infra;

namespace SME.SR.HtmlPdf
{
    public class PdfGenerator : IReportConverter
    {

        public readonly IConverter converter;

        public PdfGenerator(IConverter converter)
        {
            this.converter = converter ?? throw new ArgumentNullException(nameof(converter));
        }

        public void Converter(string html, string nomeArquivo, string tituloRelatorioRodape = "", EnumTipoDePaginacao tipoDePaginacao = EnumTipoDePaginacao.PaginaComTotalPaginas, string templateHeader = "" )
        {
            nomeArquivo = String.Format("{0}.pdf", nomeArquivo);

            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4,
                    Margins = new MarginSettings() { Top = 5, Bottom = 5, Left = 5, Right = 5 },
                    Out=nomeArquivo
                }
            };

            if (tipoDePaginacao == EnumTipoDePaginacao.SemPagina)
                doc.Objects.Add(new ObjectSettings()
                {
                    HtmlContent = html,
                    WebSettings = { DefaultEncoding = "utf-8" } ,
                    PagesCount = true
                });
            else
            {
                doc.Objects.Add(new ObjectSettings()
                {
                    HtmlContent = html,
                    WebSettings = { DefaultEncoding = "utf-8" },
                    PagesCount = true,
                    HeaderSettings = { HtmUrl = templateHeader },
                    FooterSettings = { 
                        FontName="Roboto", 
                        FontSize = 9, 
                        Right = tipoDePaginacao == EnumTipoDePaginacao.PaginaComTotalPaginas ? "[page] / [toPage]" : "[page]", 
                        Left = tituloRelatorioRodape != "" ? $"SGP - Sistema de Gestão Pedagógica | {tituloRelatorioRodape}" : "",
                    }
                }); 
            }

            converter.Convert(doc);            
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
        public void ConvertToPdfPaginacaoSolo(List<PaginaParaRelatorioPaginacaoSoloDto> paginas, string caminhoBase, string nomeArquivo, string tituloRelatorioRodape = "", Orientation orientacaoRelatorio = Orientation.Portrait)
        {
            HtmlToPdfDocument doc = StartBasicDocPaginacaoSolo(paginas, tituloRelatorioRodape, orientacaoRelatorio);

            if (!string.IsNullOrWhiteSpace(nomeArquivo))
            {
                nomeArquivo = String.Format("{0}.pdf", nomeArquivo);

                if (!string.IsNullOrWhiteSpace(caminhoBase))
                {
                    SentrySdk.AddBreadcrumb($"Caminho arquivo de relatório: {Path.Combine(caminhoBase, $"relatorios", nomeArquivo)}");
                    doc.GlobalSettings.Out = Path.Combine(caminhoBase, nomeArquivo);
                }
                else
                {
                    doc.GlobalSettings.Out = nomeArquivo;
                }
            }

            converter.Convert(doc);
            doc = null;
            GC.Collect();
        }
        private HtmlToPdfDocument StartBasicDocPaginacaoSolo(List<PaginaParaRelatorioPaginacaoSoloDto> paginas, string tituloRelatorioRodape = "", Orientation orientacaoRelatorio = Orientation.Portrait)
        {
            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                    ColorMode = ColorMode.Color,
                    Orientation = orientacaoRelatorio,
                    PaperSize = PaperKind.A4,
                    Margins = new MarginSettings() { Top = 5, Bottom = 5, Left = 5, Right = 5 }
                }
            };


            foreach (var pagina in paginas)
            {
                doc.Objects.Add(new ObjectSettings()
                {
                    HtmlContent = pagina.Html,
                    WebSettings = { DefaultEncoding = "utf-8" },
                    FooterSettings = {
                    FontName="Roboto Mono",
                    FontSize = 9, Right = $"{pagina.Pagina} / {pagina.Total}",
                    Left = !string.IsNullOrEmpty(tituloRelatorioRodape) ? $"SGP - Sistema de Gestão Pedagógica | {tituloRelatorioRodape}" : "",
                }
                });
            }

            
            return doc;
        }
        private HtmlToPdfDocument StartBasicDoc(List<string> paginas)
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
