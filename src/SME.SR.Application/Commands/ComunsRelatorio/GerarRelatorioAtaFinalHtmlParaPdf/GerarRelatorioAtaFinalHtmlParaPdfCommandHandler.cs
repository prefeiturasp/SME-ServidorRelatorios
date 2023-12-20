using DinkToPdf.Contracts;
using MediatR;
using SME.SR.HtmlPdf;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
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
        private readonly IMediator mediator;
        private Guid sessionId;

        public GerarRelatorioAtaFinalHtmlParaPdfCommandHandler(
                                                               IConverter converter,
                                                               IServicoFila servicoFila,
                                                               IHtmlHelper htmlHelper,
                                                               IMediator mediator)
        {
            this.converter = converter;
            this.servicoFila = servicoFila ?? throw new ArgumentNullException(nameof(servicoFila));
            this.htmlHelper = htmlHelper ?? throw new ArgumentNullException(nameof(htmlHelper));
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<bool> Handle(GerarRelatorioAtaFinalHtmlParaPdfCommand request, CancellationToken cancellationToken)
        {
            sessionId = request.SessionId;
            List<string> paginasEmHtml = new List<string>();

            await Logar("Gerando Paginas Razor");
            foreach (var modelPagina in request.Paginas)
            {
                string html = await htmlHelper.RenderRazorViewToString(request.NomeTemplate, modelPagina);

                html = html.Replace("logoMono.png", SmeConstants.LogoSmeMono);
                html = html.Replace("logo.png", SmeConstants.LogoSme);

                paginasEmHtml.Add(html);
                await Logar($"Pagina {request.Paginas.IndexOf(modelPagina)+1}/{request.Paginas.Count} gerada");
            }

            //TODO: FILA PARA RELATORIO SEM DADOS?
            await Logar("Gerando Paginas Razor");
            if (paginasEmHtml.Any())
            {
                PdfGenerator pdfGenerator = new PdfGenerator(converter);

                var directory = AppDomain.CurrentDomain.BaseDirectory;

                await Logar("Convertendo PDF");
                pdfGenerator.ConvertToPdf(paginasEmHtml, directory, request.CodigoCorrelacao.ToString());
                await Logar("PDF Convertido");

                await servicoFila.PublicaFila(new PublicaFilaDto(new MensagemRelatorioProntoDto(request.MensagemUsuario, string.Empty), RotasRabbitSGP.RotaRelatoriosProntosSgp, ExchangeRabbit.Sgp, request.CodigoCorrelacao));
            }

            return true;
        }

        private Task Logar(string mensagem)
            => mediator.Send(new SalvarLogViaRabbitCommand($"{sessionId}:3: {mensagem}", LogNivel.Informacao));

    }
}
