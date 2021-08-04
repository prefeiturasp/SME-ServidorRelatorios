using DinkToPdf.Contracts;
using MediatR;
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
            var paginas = new List<PaginaParaRelatorioPaginacaoSoloDto>();

            var model = (RelatorioAcompanhamentoAprendizagemDto)request.Model;

            var nomeTemplateCabecalho = "RelatorioAcompanhamentoAprendizagemCabecalho";
            var nomeTemplateCorpo = "RelatorioAcompanhamentoAprendizagemCorpo";

            var htmlCabecalho = await htmlHelper.RenderRazorViewToString(nomeTemplateCabecalho, model.Cabecalho);
            htmlCabecalho = htmlCabecalho.Replace("logoMono.png", SmeConstants.LogoSmeMono);
            htmlCabecalho = htmlCabecalho.Replace("logo.png", SmeConstants.LogoSme);

            var alunos = model.Alunos.OrderBy(a => a.NomeEol);

            foreach (var aluno in alunos)
            {
                var htmlCorpo = await htmlHelper.RenderRazorViewToString(nomeTemplateCorpo, aluno);

                var paginasDoAluno = htmlCorpo.Split("<div style='page-break-before:always'></div>");
                var iNumPagina = 1;
                if (paginasDoAluno.Length > 0)
                {
                    foreach (var paginaDoAluno in paginasDoAluno)
                    {
                        var htmlParaIncluir = htmlCabecalho.Replace("#CONTEUDO_ALUNO", paginaDoAluno);
                        paginas.Add(new PaginaParaRelatorioPaginacaoSoloDto(htmlParaIncluir, iNumPagina, paginasDoAluno.Length));
                        iNumPagina++;
                    }
                }
                else
                {
                    var htmlParaIncluir = htmlCabecalho.Replace("#CONTEUDO_ALUNO", htmlCorpo);
                    paginas.Add(new PaginaParaRelatorioPaginacaoSoloDto(htmlParaIncluir, iNumPagina, paginasDoAluno.Length));
                }
            }

            var caminhoBase = AppDomain.CurrentDomain.BaseDirectory;
            var nomeArquivo = Path.Combine(caminhoBase, "relatorios");

            //PdfGenerator pdfGenerator = new PdfGenerator(converter);
            reportConverter.ConvertToPdfPaginacaoSolo(paginas, nomeArquivo, request.CodigoCorrelacao.ToString());
            
            if (request.EnvioPorRabbit)
            {
                servicoFila.PublicaFila(new PublicaFilaDto(new MensagemRelatorioProntoDto(request.MensagemUsuario, request.MensagemTitulo), RotasRabbit.RotaRelatoriosProntosSgp, ExchangeRabbit.ExchangeSgp, request.CodigoCorrelacao));
                return string.Empty;
            }

            return request.CodigoCorrelacao.ToString();

        }
    }
}
