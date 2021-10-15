using DinkToPdf.Contracts;
using HtmlAgilityPack;
using MediatR;
using SME.SR.HtmlPdf;
using SME.SR.Infra;
using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application.Commands.ComunsRelatorio.GerarRelatorioHtmlParaPdf
{
    public class GerarRelatorioHtmlParaPdfCommandCommandHandler : IRequestHandler<GerarRelatorioHtmlParaPdfCommand, string>
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

        public async Task<string> Handle(GerarRelatorioHtmlParaPdfCommand request, CancellationToken cancellationToken)
        {
            var html = await htmlHelper.RenderRazorViewToString(request.NomeTemplate, request.Model);
            html = html.Replace("logoMono.png", SmeConstants.LogoSmeMono);
            html = html.Replace("logo.png", SmeConstants.LogoSme);
            
            var caminhoBase = AppDomain.CurrentDomain.BaseDirectory;
            var nomeArquivo = Path.Combine(caminhoBase, "relatorios", request.CodigoCorrelacao.ToString());

            //HttpWebRequest req = (HttpWebRequest)WebRequest.Create("http://google.com");
            //HttpWebResponse response = (HttpWebResponse)req.GetResponse();
            //Stream responseStream = response.GetResponseStream();
            //StreamReader streamReader = new StreamReader(responseStream);
            //var siteContent = streamReader.ReadToEnd();

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            PdfGenerator pdfGenerator = new PdfGenerator(converter);
            pdfGenerator.Converter(html, nomeArquivo, request.TituloRelatorioRodape, request.GerarPaginacao);

            if (request.EnvioPorRabbit)
            {
                await servicoFila.PublicaFila(new PublicaFilaDto(new MensagemRelatorioProntoDto(request.MensagemUsuario, request.MensagemTitulo), RotasRabbitSGP.RotaRelatoriosProntosSgp, ExchangeRabbit.Sgp, request.CodigoCorrelacao));
                return string.Empty;
            }
            else return request.CodigoCorrelacao.ToString();
        }
    }
}
