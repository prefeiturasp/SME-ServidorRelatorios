using DinkToPdf.Contracts;
using MediatR;
using SME.SR.HtmlPdf;
using SME.SR.Infra;
using SME.SR.Infra.Dtos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class GerarRelatorioHtmlPDFHistoricoEscolarCommandHandler : AsyncRequestHandler<GerarRelatorioHtmlPDFHistoricoEscolarCommand>
    {
        private readonly IConverter converter;
        private readonly IHtmlHelper htmlHelper;
        private readonly IServicoFila servicoFila;

        public GerarRelatorioHtmlPDFHistoricoEscolarCommandHandler(
                                                   IConverter converter,
                                                   IHtmlHelper htmlHelper,
                                                   IServicoFila servicoFila)
        {
            this.converter = converter ?? throw new ArgumentNullException(nameof(converter));
            this.htmlHelper = htmlHelper ?? throw new ArgumentNullException(nameof(htmlHelper));
            this.servicoFila = servicoFila ?? throw new ArgumentNullException(nameof(servicoFila));
        }

        protected override async Task Handle(GerarRelatorioHtmlPDFHistoricoEscolarCommand request, CancellationToken cancellationToken)
        {
            var paginasSolo = new List<PaginaParaRelatorioPaginacaoSoloDto>();

            foreach (var relatorio in request.Relatorios)
            {
                foreach (var relatorioPagina in relatorio.RelatorioPaginadados)
                {
                    paginasSolo.Add(await GerarPagina(relatorioPagina, relatorioPagina.PaginaAtual, relatorio.TotalPagina));
                }
            }

            var pdfGenerator = new PdfGenerator(converter);
            var caminhoBase = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "relatorios");

            pdfGenerator.ConvertToPdfPaginacaoSolo(paginasSolo, caminhoBase, request.CodigoCorrelacao.ToString(), "Histórico Escolar");

            await servicoFila.PublicaFila(new PublicaFilaDto(new MensagemRelatorioProntoDto(), RotasRabbitSGP.RotaRelatoriosProntosSgp, ExchangeRabbit.Sgp, request.CodigoCorrelacao));
        }

        private async Task<PaginaParaRelatorioPaginacaoSoloDto> GerarPagina(RelatorioPaginadoHistoricoEscolarDto relatorio, int pagina, int totalPagina)
        {
            var html = await htmlHelper.RenderRazorViewToString("HistoricoEscolar/Index", relatorio);
            html = html.Replace("logoMono.png", SmeConstants.LogoSmeMono);
            html = html.Replace("logo.png", SmeConstants.LogoSme);

            return new PaginaParaRelatorioPaginacaoSoloDto(html, pagina, totalPagina);
        }
    }
}
