using DinkToPdf.Contracts;
using MediatR;
using SME.SR.HtmlPdf;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application.Commands.ComunsRelatorio.GerarRelatorioHtmlParaPdf
{
    public class GerarRelatorioHtmlParaPdfCommandHandler : IRequestHandler<GerarRelatorioHtmlParaPdfCommand, bool>
    {
        private readonly IConverter converter;

        public GerarRelatorioHtmlParaPdfCommandHandler(IConverter converter)
        {
            this.converter = converter;
        }

        public async Task<bool> Handle(GerarRelatorioHtmlParaPdfCommand request, CancellationToken cancellationToken)
        {
            try
            {
                List<string> paginasEmHtml = new List<string>();

                foreach (var modelPagina in request.Paginas)
                {
                    string html = string.Empty;

                    html = GerarHtmlRazor(modelPagina, request.NomeTemplate);

                    html = html.Replace("logo.png", SmeConstants.LogoSme);

                    paginasEmHtml.Add(html);
                }

                PdfGenerator pdfGenerator = new PdfGenerator(converter);

                var directory = AppDomain.CurrentDomain.BaseDirectory;

                pdfGenerator.ConvertToPdf(paginasEmHtml, directory, request.CodigoCorrelacao.ToString());

                byte[] bytes = pdfGenerator.ConvertToPdf(paginasEmHtml);

                return true;
            }
            catch (Exception)
            {
                // TODO sentry?
                return false;
            }
        }

        private string GerarHtmlRazor<T>(T model, string nomeDoArquivoDoTemplate)
        {
            string templateBruto = System.IO.File.ReadAllText(nomeDoArquivoDoTemplate);

            RazorProcessor processor = new RazorProcessor();

            string templateProcessado = processor.ProcessarTemplate(model, templateBruto, nomeDoArquivoDoTemplate);

            return templateProcessado;
        }
    }
}
