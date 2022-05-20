using DinkToPdf.Contracts;
using MediatR;
using SME.SR.HtmlPdf;
using SME.SR.Infra;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application.Commands.ComunsRelatorio.GerarRelatorioHtmlParaPdf
{
    public class GerarRelatorioHtmlParaPdfCommandCommandHandler : IRequestHandler<GerarRelatorioHtmlParaPdfCommand, string>
    {
        private readonly IConverter converter;
        private readonly IServicoFila servicoFila;
        private readonly IHtmlHelper htmlHelper;
        private readonly IMediator mediator;

        public GerarRelatorioHtmlParaPdfCommandCommandHandler(IConverter converter,
                                                              IServicoFila servicoFila,
                                                              IHtmlHelper htmlHelper,
                                                              IMediator mediator)
        {
            this.converter = converter;
            this.servicoFila = servicoFila ?? throw new ArgumentNullException(nameof(servicoFila));
            this.htmlHelper = htmlHelper ?? throw new ArgumentNullException(nameof(htmlHelper));
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<string> Handle(GerarRelatorioHtmlParaPdfCommand request, CancellationToken cancellationToken)
        {
            var html = await htmlHelper.RenderRazorViewToString(request.NomeTemplate, request.Model);
            html = html.Replace("logoMono.png", SmeConstants.LogoSmeMono);
            html = html.Replace("logo.png", SmeConstants.LogoSme);

            var caminhoBase = AppDomain.CurrentDomain.BaseDirectory;
            string nomeArquivo;

            if (request.RelatorioSincrono)
                nomeArquivo = Path.Combine(caminhoBase, "relatoriossincronos", request.DiretorioComplementar, request.CodigoCorrelacao.ToString());
            else
                nomeArquivo = Path.Combine(caminhoBase, "relatorios", request.DiretorioComplementar, request.CodigoCorrelacao.ToString());

            await mediator.Send(new SalvarLogViaRabbitCommand($"GerarRelatorioHtmlParaPdfCommand - Caminho arquivo: {nomeArquivo}", LogNivel.Informacao));

            PdfGenerator pdfGenerator = new PdfGenerator(converter);
            pdfGenerator.Converter(html, nomeArquivo, request.TituloRelatorioRodape, request.GerarPaginacao);

            if (request.EnvioPorRabbit)
            {
                await servicoFila.PublicaFila(new PublicaFilaDto(new MensagemRelatorioProntoDto(request.MensagemUsuario, request.MensagemTitulo),
                    RotasRabbitSGP.RotaRelatoriosProntosSgp, ExchangeRabbit.Sgp, request.CodigoCorrelacao));

                return string.Empty;
            }
            else
            {
                return request.CodigoCorrelacao.ToString();
            }
        }
    }
}
