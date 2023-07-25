using DinkToPdf.Contracts;
using MediatR;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Sentry;
using SME.SR.HtmlPdf;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class GerarRelatorioAtaFinalHtmlParaPdfCommandHandler : IRequestHandler<GerarRelatorioAtaFinalHtmlParaPdfCommand, bool>
    {
        private readonly IConverter converter;
        private readonly IServicoFila servicoFila;
        private readonly IHtmlHelper htmlHelper;

        public GerarRelatorioAtaFinalHtmlParaPdfCommandHandler(
                                                               IConverter converter,
                                                               IServicoFila servicoFila,
                                                               IHtmlHelper htmlHelper)
        {
            this.converter = converter;
            this.servicoFila = servicoFila ?? throw new ArgumentNullException(nameof(servicoFila));
            this.htmlHelper = htmlHelper ?? throw new ArgumentNullException(nameof(htmlHelper));
        }

        public async Task<bool> Handle(GerarRelatorioAtaFinalHtmlParaPdfCommand request, CancellationToken cancellationToken)
        {
            List<string> paginasEmHtml = new List<string>();

            foreach (var modelPagina in request.Paginas)
            {
                string html = await htmlHelper.RenderRazorViewToString(request.NomeTemplate, modelPagina);

                html = html.Replace("logoMono.png", SmeConstants.LogoSmeMono);
                html = html.Replace("logo.png", SmeConstants.LogoSme);

                paginasEmHtml.Add(html);
            }

            //TODO: FILA PARA RELATORIO SEM DADOS?
            if (paginasEmHtml.Any())
            {
                PdfGenerator pdfGenerator = new PdfGenerator(converter);

                var directory = AppDomain.CurrentDomain.BaseDirectory;

                pdfGenerator.ConvertToPdf(paginasEmHtml, directory, request.CodigoCorrelacao.ToString());

                await servicoFila.PublicaFila(new PublicaFilaDto(new MensagemRelatorioProntoDto(request.MensagemUsuario, string.Empty), RotasRabbitSGP.RotaRelatoriosProntosSgp, ExchangeRabbit.Sgp, request.CodigoCorrelacao));
            }

            return true;
        }
    }
}
