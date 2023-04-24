using DinkToPdf.Contracts;
using MediatR;
using SME.SR.HtmlPdf;
using SME.SR.Infra;
using SME.SR.Infra.Dtos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class GerarRelatorioHtmlPDFEncaminhamentoNaapaCommadHandler : AsyncRequestHandler<GerarRelatorioHtmlPDFEncaminhamentoNaapaCommad>
    {
        private readonly IConverter converter;
        private readonly IHtmlHelper htmlHelper;
        private readonly IServicoFila servicoFila;

        public GerarRelatorioHtmlPDFEncaminhamentoNaapaCommadHandler(
                                                           IConverter converter,
                                                           IHtmlHelper htmlHelper,
                                                           IServicoFila servicoFila)
        {
            this.converter = converter ?? throw new ArgumentNullException(nameof(converter));
            this.htmlHelper = htmlHelper ?? throw new ArgumentNullException(nameof(htmlHelper));
            this.servicoFila = servicoFila ?? throw new ArgumentNullException(nameof(servicoFila));
        }

        protected override async Task Handle(GerarRelatorioHtmlPDFEncaminhamentoNaapaCommad request, CancellationToken cancellationToken)
        {
            var paginasSolo = new List<PaginaParaRelatorioPaginacaoSoloDto>();
            var relatorioPaginado = new EncaminhamentoNaapaPaginado(request.Relatorio);
            var paginas = relatorioPaginado.ObterPaginas();
            var contadorPagina = 1;

            foreach (var pagina in paginas)
            {
                paginasSolo.Add(await GerarPagina(pagina, contadorPagina, paginas.Count()));
                contadorPagina++;
            }

            var pdfGenerator = new PdfGenerator(converter);
            var caminhoBase = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "relatorios");
            pdfGenerator.ConvertToPdfPaginacaoSolo(paginasSolo, caminhoBase, request.CodigoCorrelacao.ToString(), "Relatório do encaminhamento NAAPA");

            await servicoFila.PublicaFila(new PublicaFilaDto(new MensagemRelatorioProntoDto(), RotasRabbitSGP.RotaRelatoriosProntosSgp, ExchangeRabbit.Sgp, request.CodigoCorrelacao));
        }

        private async Task<PaginaParaRelatorioPaginacaoSoloDto> GerarPagina(RelatorioEncaminhamentosNAAPADto relatorio, int pagina, int totalPaginas)
        {
            var html = await htmlHelper.RenderRazorViewToString("RelatorioEncaminhamentosNAAPA", relatorio);
            html = html.Replace("logoMono.png", SmeConstants.LogoSmeMono);
            html = html.Replace("logo.png", SmeConstants.LogoSme);

            return new PaginaParaRelatorioPaginacaoSoloDto(html, pagina, totalPaginas);
        }
    }
}
