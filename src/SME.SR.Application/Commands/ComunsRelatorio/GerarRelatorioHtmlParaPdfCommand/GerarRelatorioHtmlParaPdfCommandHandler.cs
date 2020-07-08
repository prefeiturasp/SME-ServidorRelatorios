using DinkToPdf.Contracts;
using MediatR;
using SME.SR.HtmlPdf;
using SME.SR.Infra;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application.Commands.ComunsRelatorio.GerarRelatorioHtmlParaPdf
{
    public class GerarRelatorioHtmlParaPdfCommandCommandHandler : IRequestHandler<GerarRelatorioHtmlParaPdfCommand, bool>
    {
        private readonly IConverter converter;
        private readonly IServicoFila servicoFila;

        public GerarRelatorioHtmlParaPdfCommandCommandHandler(IConverter converter,
                                                              IServicoFila servicoFila)
        {
            this.converter = converter;
            this.servicoFila = servicoFila ?? throw new ArgumentNullException(nameof(servicoFila));
        }

        public async Task<bool> Handle(GerarRelatorioHtmlParaPdfCommand request, CancellationToken cancellationToken)
        {
            var html = GerarHtmlRazor(request.Model, request.NomeTemplate);
            var caminhoBase = AppDomain.CurrentDomain.BaseDirectory;
            var nomeArquivo = Path.Combine(caminhoBase, "relatorios", request.CodigoCorrelacao.ToString());

            PdfGenerator pdfGenerator = new PdfGenerator(converter);
            pdfGenerator.Converter(html, nomeArquivo);

            servicoFila.PublicaFila(new PublicaFilaDto(null, RotasRabbit.FilaSgp, RotasRabbit.RotaRelatoriosProntosSgp, null, request.CodigoCorrelacao));

            return true;
        }

        private string GerarHtmlRazor(object model, string nomeDoArquivoDoTemplate)
        {
            var caminhoBase = AppDomain.CurrentDomain.BaseDirectory;
            var nomeArquivo = $"wwwroot/templates/{nomeDoArquivoDoTemplate}";
            var caminhoArquivo = Path.Combine($"{caminhoBase}", nomeArquivo);

            var templateBruto = File.ReadAllLines(caminhoArquivo);

            var html = string.Join("", templateBruto.Skip(templateBruto.Count(c => c.Contains("@model"))));

            RazorProcessor processor = new RazorProcessor();

            string templateProcessado = processor.ProcessarTemplate(model, html, nomeDoArquivoDoTemplate);

            return templateProcessado;
        }
    }
}
