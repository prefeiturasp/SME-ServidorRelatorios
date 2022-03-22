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
    public class GerarRelatorioHtmlCommandCommandHandler : IRequestHandler<GerarRelatorioHtmlCommand, string>
    {
        private readonly IConverter converter;
        private readonly IServicoFila servicoFila;
        private readonly IHtmlHelper htmlHelper;

        public GerarRelatorioHtmlCommandCommandHandler(IConverter converter,
            IServicoFila servicoFila,
            IHtmlHelper htmlHelper)
        {
            this.converter = converter;
            this.servicoFila = servicoFila ?? throw new ArgumentNullException(nameof(servicoFila));
            this.htmlHelper = htmlHelper ?? throw new ArgumentNullException(nameof(htmlHelper));
        }

        public async Task<string> Handle(GerarRelatorioHtmlCommand request, CancellationToken cancellationToken)
        {
            var html = await htmlHelper.RenderRazorViewToString(request.NomeTemplate, request.Model);
            html = html.Replace("logoMono.png", SmeConstants.LogoSmeMono);
            html = html.Replace("logo.png", SmeConstants.LogoSme);

            var caminhoBase = AppDomain.CurrentDomain.BaseDirectory;
            var nomeArquivo = Path.Combine(caminhoBase, "relatorios", $"{request.CodigoCorrelacao}.html");
            var nomeDiretorio = Path.GetDirectoryName(nomeArquivo);

            if (!Directory.Exists(nomeDiretorio))
                Directory.CreateDirectory(nomeDiretorio);
            
            File.WriteAllText(nomeArquivo, html);
            
            if (!request.EnvioPorRabbit)
                return request.CodigoCorrelacao.ToString();

            await servicoFila.PublicaFila(new PublicaFilaDto(
                new MensagemRelatorioProntoDto(request.MensagemUsuario, request.MensagemTitulo,mensagemDados:request.MensagemDados),
                request.NomeFila, ExchangeRabbit.Sgp, request.CodigoCorrelacao));
            return string.Empty;
        }
    }
}