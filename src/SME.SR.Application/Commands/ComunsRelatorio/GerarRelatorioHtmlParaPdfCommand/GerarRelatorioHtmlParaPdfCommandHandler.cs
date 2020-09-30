﻿using DinkToPdf.Contracts;
using MediatR;
using SME.SR.HtmlPdf;
using SME.SR.Infra;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application.Commands.ComunsRelatorio.GerarRelatorioHtmlParaPdf
{
    public class GerarRelatorioHtmlParaPdfCommandCommandHandler : IRequestHandler<GerarRelatorioHtmlParaPdfCommand, bool>
    {
        private readonly IConverter converter;
        private readonly IServicoFila servicoFila;
        private readonly IHtmlHelper htmlHelper;

        public GerarRelatorioHtmlParaPdfCommandCommandHandler(IConverter converter,
                                                              IServicoFila servicoFila,
                                                              IHtmlHelper htmlHelper)
        {
            this.converter = converter;
            this.servicoFila = servicoFila ?? throw new ArgumentNullException(nameof(servicoFila));
            this.htmlHelper = htmlHelper ?? throw new ArgumentNullException(nameof(htmlHelper));
        }

        public async Task<bool> Handle(GerarRelatorioHtmlParaPdfCommand request, CancellationToken cancellationToken)
        {
            var html = await htmlHelper.RenderRazorViewToString(request.NomeTemplate, request.Model);
            html = html.Replace("logoMono.png", SmeConstants.LogoSmeMono);
            html = html.Replace("logo.png", SmeConstants.LogoSme);

            var caminhoBase = AppDomain.CurrentDomain.BaseDirectory;
            var nomeArquivo = Path.Combine(caminhoBase, "relatorios", request.CodigoCorrelacao.ToString());

            PdfGenerator pdfGenerator = new PdfGenerator(converter);
            pdfGenerator.Converter(html, nomeArquivo);

            servicoFila.PublicaFila(new PublicaFilaDto(new MensagemRelatorioProntoDto(request.MensagemUsuario), RotasRabbit.FilaSgp, RotasRabbit.RotaRelatoriosProntosSgp, null, request.CodigoCorrelacao));

            return true;
        }
    }
}
