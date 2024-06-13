using DinkToPdf.Contracts;
using MediatR;
using SME.SR.HtmlPdf;
using SME.SR.Infra.Dtos;
using SME.SR.Infra.Dtos.Relatorios.Conecta;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class GerarRelatorioHtmlPDFLaudaCompletaCommandHandler : IRequestHandler<GerarRelatorioHtmlPDFLaudaCompletaCommand, string>
    {
        private readonly IConverter converter;
        private readonly IHtmlHelper htmlHelper;

        public GerarRelatorioHtmlPDFLaudaCompletaCommandHandler(
                                                           IConverter converter,
                                                           IHtmlHelper htmlHelper)
        {
            this.converter = converter ?? throw new ArgumentNullException(nameof(converter));
            this.htmlHelper = htmlHelper ?? throw new ArgumentNullException(nameof(htmlHelper));
        }

        public async Task<string> Handle(GerarRelatorioHtmlPDFLaudaCompletaCommand request, CancellationToken cancellationToken)
        {
            var paginasSolo = new List<PaginaParaRelatorioPaginacaoSoloDto>();
            var nomeRelatorio = Guid.NewGuid().ToString();

            foreach (var pagina in request.RelatorioPaginado.Paginas)
            {
                paginasSolo.Add(await GerarPagina(pagina, pagina.Pagina, request.RelatorioPaginado.TotalPagina));
            }


            var caminhoBase = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "relatorios");
            var pdfGenerator = new PdfGenerator(converter);

            pdfGenerator.ConvertToPdfPaginacaoSolo(paginasSolo, caminhoBase, nomeRelatorio, "Relatório de Proposta - Lauda completa");

            return nomeRelatorio;
        }

        private async Task<PaginaParaRelatorioPaginacaoSoloDto> GerarPagina(RelatorioPaginaLaudaCompletaDto relatorio, int pagina, int totalPaginas)
        {
            var html = await htmlHelper.RenderRazorViewToString("RelatorioPropostaLaudaCompleta", relatorio);
            html = html.Replace("logoMono.png", SmeConstants.LogoSmeMono);
            html = html.Replace("logo.png", SmeConstants.LogoSme);

            return new PaginaParaRelatorioPaginacaoSoloDto(html, pagina, totalPaginas);
        }
    }
}
