using DinkToPdf.Contracts;
using MediatR;
using Sentry;
using SME.SR.HtmlPdf;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application.Commands.ComunsRelatorio.GerarRelatorioHtmlParaPdf
{
    public class GerarRelatorioAtaFinalHtmlParaPdfCommandHandler : IRequestHandler<GerarRelatorioAtaFinalHtmlParaPdfCommand, bool>
    {
        private readonly IConverter converter;
        private readonly IServicoFila servicoFila;

        public GerarRelatorioAtaFinalHtmlParaPdfCommandHandler(IConverter converter,
                                                       IServicoFila servicoFila)
        {
            this.converter = converter;
            this.servicoFila = servicoFila ?? throw new ArgumentNullException(nameof(servicoFila));
        }

        public async Task<bool> Handle(GerarRelatorioAtaFinalHtmlParaPdfCommand request, CancellationToken cancellationToken)
        {
            List<string> paginasEmHtml = new List<string>();

            foreach (var modelPagina in request.Paginas)
            {
                string html = string.Empty;

                html = GerarHtmlRazor(modelPagina, request.NomeTemplate);

                html = html.Replace("logo.png", SmeConstants.LogoSme);

                paginasEmHtml.Add(html);
            }
            
            //TODO: FILA PARA RELATORIO SEM DADOS?
            if (paginasEmHtml.Any())
            {

                PdfGenerator pdfGenerator = new PdfGenerator(converter);

                var directory = AppDomain.CurrentDomain.BaseDirectory;
                SentrySdk.AddBreadcrumb($"Gerando PDF", $"Caminho geração : {directory}");

                pdfGenerator.ConvertToPdf(paginasEmHtml, directory, request.CodigoCorrelacao.ToString());

                SentrySdk.AddBreadcrumb($"Indo publicar na fila Prontos..", "8 - MonitorarStatusRelatorioUseCase");
                servicoFila.PublicaFila(new PublicaFilaDto(null, RotasRabbit.FilaSgp, RotasRabbit.RotaRelatoriosProntosSgp, null, request.CodigoCorrelacao));
                SentrySdk.CaptureMessage("8 - MonitorarStatusRelatorioUseCase - Publicado na fila PRONTO OK!");
            }

            return true;
        }

        private string GerarHtmlRazor<T>(T model, string nomeDoArquivoDoTemplate)
        {
            //TODO TRATRAR EM AMBIENTE DE DESENVOLVIMENTO PARA REMOVER SME.SR.Workers.SGP
            var caminhoBase = AppDomain.CurrentDomain.BaseDirectory;
            var nomeArquivo = $"wwwroot/templates/{nomeDoArquivoDoTemplate}";
            var caminhoArquivo = Path.Combine($"{caminhoBase}", nomeArquivo);

            SentrySdk.AddBreadcrumb($"Caminho arquivo cshtml: {caminhoArquivo}");

            string templateBruto = File.ReadAllText(caminhoArquivo);

            SentrySdk.AddBreadcrumb($"Leu arquivo de template.");

            RazorProcessor processor = new RazorProcessor();

            string templateProcessado = processor.ProcessarTemplate(model, templateBruto, nomeDoArquivoDoTemplate);

            return templateProcessado;
        }
    }
}
