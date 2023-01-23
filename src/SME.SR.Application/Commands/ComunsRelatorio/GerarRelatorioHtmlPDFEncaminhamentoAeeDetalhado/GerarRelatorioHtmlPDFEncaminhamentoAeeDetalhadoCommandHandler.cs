using DinkToPdf.Contracts;
using MediatR;
using SME.SR.HtmlPdf;
using SME.SR.Infra;
using SME.SR.Infra.Dtos;
using SME.SR.Infra.RelatorioPaginado;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class GerarRelatorioHtmlPDFEncaminhamentoAeeDetalhadoCommandHandler : AsyncRequestHandler<GerarRelatorioHtmlPDFEncaminhamentoAeeDetalhadoCommand>
    {
        private readonly IConverter converter;
        private readonly IHtmlHelper htmlHelper;
        private readonly IServicoFila servicoFila;

        public GerarRelatorioHtmlPDFEncaminhamentoAeeDetalhadoCommandHandler(
                                                           IConverter converter,
                                                           IHtmlHelper htmlHelper,
                                                           IServicoFila servicoFila)
        {
            this.converter = converter ?? throw new ArgumentNullException(nameof(converter));
            this.htmlHelper = htmlHelper ?? throw new ArgumentNullException(nameof(htmlHelper));
            this.servicoFila = servicoFila ?? throw new ArgumentNullException(nameof(servicoFila));
        }

        protected override async Task Handle(GerarRelatorioHtmlPDFEncaminhamentoAeeDetalhadoCommand request, CancellationToken cancellationToken)
        {

        }


        private async Task<PaginaParaRelatorioPaginacaoSoloDto> GerarPagina(RelatorioEncaminhamentoAeeDetalhadoDto relatorio, int pagina, int totalPaginas)
        {
            var html = await htmlHelper.RenderRazorViewToString("RelatorioEncaminhamentoAEEDetalhado", relatorio);
            html = html.Replace("logoMono.png", SmeConstants.LogoSmeMono);
            html = html.Replace("logo.png", SmeConstants.LogoSme);

            return new PaginaParaRelatorioPaginacaoSoloDto(html, pagina, totalPaginas);
        }
    }
}
