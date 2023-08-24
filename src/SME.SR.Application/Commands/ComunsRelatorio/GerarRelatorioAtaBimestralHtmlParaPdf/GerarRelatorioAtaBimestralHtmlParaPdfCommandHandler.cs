using DinkToPdf.Contracts;
using MediatR;
using Sentry;
using SME.SR.HtmlPdf;
using SME.SR.Infra;
using SME.SR.Infra.Dtos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class GerarRelatorioAtaBimestralHtmlParaPdfCommandHandler : IRequestHandler<GerarRelatorioAtaBimestralHtmlParaPdfCommand, bool>
    {
        private readonly IConverter converter;
        private readonly IServicoFila servicoFila;

        public GerarRelatorioAtaBimestralHtmlParaPdfCommandHandler(IConverter converter,
                                                       IServicoFila servicoFila)
        {
            this.converter = converter;
            this.servicoFila = servicoFila ?? throw new ArgumentNullException(nameof(servicoFila));
        }

        public async Task<bool> Handle(GerarRelatorioAtaBimestralHtmlParaPdfCommand request, CancellationToken cancellationToken)
        {
            List<PaginaParaRelatorioPaginacaoSoloDto> paginasEmHtml = new List<PaginaParaRelatorioPaginacaoSoloDto>();
            var pagina = 1;

            foreach (var modelPagina in request.Paginas)
            {
                string html = string.Empty;

                html = GerarHtmlRazor(modelPagina, request.NomeTemplate);

                html = html.Replace("logoMono.png", SmeConstants.LogoSmeMono);
                html = html.Replace("logo.png", SmeConstants.LogoSme);

                paginasEmHtml.Add(new PaginaParaRelatorioPaginacaoSoloDto(html, pagina, request.Paginas.Count()));

                pagina++;
            }           
            
            if (paginasEmHtml.Any())
            {

                PdfGenerator pdfGenerator = new PdfGenerator(converter);

                var caminhoBase = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "relatorios");

                pdfGenerator.ConvertToPdfPaginacaoSolo(paginasEmHtml, caminhoBase, request.CodigoCorrelacao.ToString(), "Relatório de Ata Bimestral");
                await servicoFila.PublicaFila(new PublicaFilaDto(new MensagemRelatorioProntoDto(request.MensagemUsuario, string.Empty), RotasRabbitSGP.RotaRelatoriosProntosSgp, ExchangeRabbit.Sgp, request.CodigoCorrelacao));                
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
