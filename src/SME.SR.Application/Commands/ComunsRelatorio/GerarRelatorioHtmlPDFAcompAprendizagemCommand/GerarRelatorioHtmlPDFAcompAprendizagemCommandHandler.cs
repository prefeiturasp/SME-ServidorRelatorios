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

namespace SME.SR.Application.Commands.ComunsRelatorio.GerarRelatorioHtmlParaPdf
{
    public class GerarRelatorioHtmlPDFAcompAprendizagemCommandHandler : IRequestHandler<GerarRelatorioHtmlPDFAcompAprendizagemCommand, string>
    {
        private readonly IServicoFila servicoFila;
        private readonly IHtmlHelper htmlHelper;
        private readonly IReportConverter reportConverter;

        public GerarRelatorioHtmlPDFAcompAprendizagemCommandHandler(IConverter converter,
                                                              IServicoFila servicoFila,
                                                              IHtmlHelper htmlHelper, IReportConverter reportConverter)
        {
            this.servicoFila = servicoFila ?? throw new ArgumentNullException(nameof(servicoFila));
            this.htmlHelper = htmlHelper ?? throw new ArgumentNullException(nameof(htmlHelper));
            this.reportConverter = reportConverter ?? throw new ArgumentNullException(nameof(reportConverter));
        }

        public async Task<string> Handle(GerarRelatorioHtmlPDFAcompAprendizagemCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var documentos = new List<string>();

                var model = request.Model;

                var nomeTemplateCabecalho = "RelatorioAcompanhamentoAprendizagemHeader";
                var nomeTemplateCorpo = "RelatorioAcompanhamentoAprendizagem";

                var html = await htmlHelper.RenderRazorViewToString(nomeTemplateCorpo, model);
                var htmlHeader = await htmlHelper.RenderRazorViewToString(nomeTemplateCabecalho, model);

                var caminhoBase = AppDomain.CurrentDomain.BaseDirectory;
                var nomeArquivo = Path.Combine(caminhoBase, "relatorios", request.CodigoCorrelacao.ToString());
                var nomeHeader = $"{nomeArquivo}.html";

                System.IO.StreamWriter file = new System.IO.StreamWriter(nomeHeader);
                file.WriteLine(htmlHeader);
                file.Close();

                reportConverter.Converter(html, nomeArquivo, templateHeader: htmlHeader);
            
                if (request.EnvioPorRabbit)
                {
                    await servicoFila.PublicaFila(new PublicaFilaDto(new MensagemRelatorioProntoDto(request.MensagemUsuario, request.MensagemTitulo), RotasRabbitSGP.RotaRelatoriosProntosSgp, ExchangeRabbit.Sgp, request.CodigoCorrelacao));
                    return string.Empty;
                }

                return request.CodigoCorrelacao.ToString();
            }
            catch (Exception e)
            {
                SentrySdk.CaptureMessage($"Erro na geração do relatório de acompanhamento de aprendizagem - {e.Message}");
                throw e;
            }

        }
    }
}
