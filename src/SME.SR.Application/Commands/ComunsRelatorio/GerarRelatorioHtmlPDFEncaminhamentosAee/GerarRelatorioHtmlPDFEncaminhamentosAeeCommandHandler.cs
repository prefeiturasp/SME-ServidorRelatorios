﻿using DinkToPdf.Contracts;
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
    public class GerarRelatorioHtmlPDFEncaminhamentosAeeCommandHandler : AsyncRequestHandler<GerarRelatorioHtmlPDFEncaminhamentosAeeCommand>
    {
        private readonly IConverter converter;
        private readonly IHtmlHelper htmlHelper;
        private readonly IServicoFila servicoFila;

        public GerarRelatorioHtmlPDFEncaminhamentosAeeCommandHandler(
                                                           IConverter converter,
                                                           IHtmlHelper htmlHelper,
                                                           IServicoFila servicoFila)
        {
            this.converter = converter ?? throw new ArgumentNullException(nameof(converter));
            this.htmlHelper = htmlHelper ?? throw new ArgumentNullException(nameof(htmlHelper));
            this.servicoFila = servicoFila ?? throw new ArgumentNullException(nameof(servicoFila));
        }

        protected override async Task Handle(GerarRelatorioHtmlPDFEncaminhamentosAeeCommand request, CancellationToken cancellationToken)
        {
            var relatorioPaginado = new RelatorioPaginadoEncaminhamentoAee(request.Cabecalho, request.Agrupamentos);
            var paginasSolo = new List<PaginaParaRelatorioPaginacaoSoloDto>();
            var paginas = relatorioPaginado.ObterRelatorioPaginado();
            var indicePagina = 0;

            foreach (var relatorio in paginas)
            {
                indicePagina++;
                paginasSolo.Add(await GerarPagina(relatorio, indicePagina, paginas.Count()));
            }

            var pdfGenerator = new PdfGenerator(converter);

            var caminhoBase = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "relatorios");
            pdfGenerator.ConvertToPdfPaginacaoSolo(paginasSolo, caminhoBase, request.CodigoCorrelacao.ToString(), "Relatório dos Encaminhamentos AEE");

            await servicoFila.PublicaFila(new PublicaFilaDto(new MensagemRelatorioProntoDto(), RotasRabbitSGP.RotaRelatoriosProntosSgp, ExchangeRabbit.Sgp, request.CodigoCorrelacao));
        }

        private async Task<PaginaParaRelatorioPaginacaoSoloDto> GerarPagina(RelatorioEncaminhamentoAeeDto relatorio, int pagina, int totalPaginas)
        {
            var html = await htmlHelper.RenderRazorViewToString("RelatorioEncaminhamentoAEE", relatorio);
            html = html.Replace("logoMono.png", SmeConstants.LogoSmeMono);
            html = html.Replace("logo.png", SmeConstants.LogoSme);

            return new PaginaParaRelatorioPaginacaoSoloDto(html, pagina, totalPaginas);
        }
    }
}
