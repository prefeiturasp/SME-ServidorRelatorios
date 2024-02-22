using DinkToPdf.Contracts;
using MediatR;
using SME.SR.HtmlPdf;
using SME.SR.Infra;
using SME.SR.Infra.Dtos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class GerarRelatorioHtmlPDFEncaminhamentoNaapaDetalhadoCommandHandler : AsyncRequestHandler<GerarRelatorioHtmlPDFEncaminhamentoNaapaDetalhadoCommand>
    {
        private readonly IReportConverter pdfGenerator;
        private readonly IHtmlHelper htmlHelper;
        private readonly IServicoFila servicoFila;
        private readonly VariaveisAmbiente variaveisAmbiente;

        public GerarRelatorioHtmlPDFEncaminhamentoNaapaDetalhadoCommandHandler(
                                                           IConverter converter,
                                                           IHtmlHelper htmlHelper,
                                                           IServicoFila servicoFila,
                                                           VariaveisAmbiente variaveisAmbiente)
        {
            this.pdfGenerator = new PdfGenerator(converter);
            this.htmlHelper = htmlHelper ?? throw new ArgumentNullException(nameof(htmlHelper));
            this.servicoFila = servicoFila ?? throw new ArgumentNullException(nameof(servicoFila));
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        protected override async Task Handle(GerarRelatorioHtmlPDFEncaminhamentoNaapaDetalhadoCommand request, CancellationToken cancellationToken)
        {
            if (request.ImprimirAnexos)
                await GerarRelatorioPdfComAnexo(request);
            else
                await GerarRelatorioPdf(request);

            await servicoFila.PublicaFila(new PublicaFilaDto(new MensagemRelatorioProntoDto(), RotasRabbitSGP.RotaRelatoriosProntosSgp, ExchangeRabbit.Sgp, request.CodigoCorrelacao));
        }

        private async Task GerarRelatorioPdf(GerarRelatorioHtmlPDFEncaminhamentoNaapaDetalhadoCommand request)
        {
            var paginasSolo = new List<PaginaParaRelatorioPaginacaoSoloDto>();

            foreach (var relatorioDto in request.RelatorioEncaminhamentoNAAPADetalhadoDtos)
            {
                var relatorioPaginas = new EncaminhamentoNaapaDetalhadoPaginado().ObterPaginas(relatorioDto);

                foreach (var pagina in relatorioPaginas)
                {
                    paginasSolo.Add(await GerarPagina(pagina, pagina.Pagina, relatorioPaginas.Count()));
                }
            }

            var caminhoBase = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "relatorios");
            pdfGenerator.ConvertToPdfPaginacaoSolo(paginasSolo, caminhoBase, request.CodigoCorrelacao.ToString(), "Relatório do encaminhamento NAAPA");
        }

        private async Task GerarRelatorioPdfComAnexo(GerarRelatorioHtmlPDFEncaminhamentoNaapaDetalhadoCommand request)
        {
            var caminhoBase = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "relatorios");
            var pdfSemAnexo = $"{request.CodigoCorrelacao.ToString()}_sem_anexo";
            var paginasSolo = new List<PaginaParaRelatorioPaginacaoSoloDto>();
            var relatorioDto = request.RelatorioEncaminhamentoNAAPADetalhadoDtos.FirstOrDefault();
            var relatorioPaginas = new EncaminhamentoNaapaDetalhadoPaginado().ObterPaginas(relatorioDto);

            foreach (var pagina in relatorioPaginas)
            {
                paginasSolo.Add(await GerarPagina(pagina, pagina.Pagina, relatorioPaginas.Count()));
            }

            pdfGenerator.ConvertToPdfPaginacaoSolo(paginasSolo, caminhoBase, pdfSemAnexo, "Relatório do encaminhamento NAAPA");

            var unificador = new UnificarPdfNAAPA(variaveisAmbiente, pdfSemAnexo, relatorioDto.AnexosPdf.ToList(), request.CodigoCorrelacao.ToString());

            unificador.Execute();
        }

        private async Task<PaginaParaRelatorioPaginacaoSoloDto> GerarPagina(EncaminhamentoNaapaDetalhadoPagina relatorio, int pagina, int totalPaginas)
        {
            var html = await htmlHelper.RenderRazorViewToString("RelatorioEncaminhamentoNaapaDetalhado", relatorio);
            html = html.Replace("logoMono.png", SmeConstants.LogoSmeMono);
            html = html.Replace("logo.png", SmeConstants.LogoSme);

            return new PaginaParaRelatorioPaginacaoSoloDto(html, pagina, totalPaginas);
        }
    }
}
