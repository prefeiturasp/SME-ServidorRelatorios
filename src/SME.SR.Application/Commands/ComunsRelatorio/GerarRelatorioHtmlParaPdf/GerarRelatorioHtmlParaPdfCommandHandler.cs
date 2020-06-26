using MediatR;
using SME.SR.HtmlPdf;
using SME.SR.Infra.Dtos.Relatorios.ConselhoClasse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application.Commands.ComunsRelatorio.GerarRelatorioHtmlParaPdf
{
    public class GerarRelatorioHtmlParaPdfCommandHandler : IRequestHandler<GerarRelatorioHtmlParaPdfCommand, bool>
    {
        public GerarRelatorioHtmlParaPdfCommandHandler()
        {

        }

        public async Task<bool> Handle(GerarRelatorioHtmlParaPdfCommand request, CancellationToken cancellationToken)
        {
            List<string> paginasEmHtml = new List<string>();

            foreach (ConselhoClasseAtaFinalPaginaDto modelPagina in request.Paginas)
            {
                string html = string.Empty;

                html = GerarHtmlRazor(modelPagina, request.NomeTemplate);

                html = html.Replace("logo.png", SmeConstants.LogoSme);

                paginasEmHtml.Add(html);
            }


            return true;
        }

        private string GerarHtmlRazor(ConselhoClasseAtaFinalPaginaDto model, string nomeDoArquivoDoTemplate)
        {
            string templateBruto = System.IO.File.ReadAllText(nomeDoArquivoDoTemplate);

            RazorProcessor processor = new RazorProcessor();

            string templateProcessado = processor.ProcessarTemplate(model, templateBruto, nomeDoArquivoDoTemplate);

            return templateProcessado;
        }
    }
}
