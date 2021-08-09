using DinkToPdf.Contracts;
using MediatR;
using SME.SR.HtmlPdf;
using SME.SR.Infra;
using SME.SR.Infra.Dtos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application.Commands.ComunsRelatorio.GerarRelatorioHtmlParaPdf
{
    public class GerarRelatorioHtmlPDFRegistroIndividualCommandHandler : IRequestHandler<GerarRelatorioHtmlPDFRegistroIndividualCommand, string>
    {
        private readonly IConverter converter;
        private readonly IServicoFila servicoFila;
        private readonly IHtmlHelper htmlHelper;

        public GerarRelatorioHtmlPDFRegistroIndividualCommandHandler(IConverter converter,
                                                              IServicoFila servicoFila,
                                                              IHtmlHelper htmlHelper)
        {
            this.converter = converter;
            this.servicoFila = servicoFila ?? throw new ArgumentNullException(nameof(servicoFila));
            this.htmlHelper = htmlHelper ?? throw new ArgumentNullException(nameof(htmlHelper));
        }

        public async Task<string> Handle(GerarRelatorioHtmlPDFRegistroIndividualCommand request, CancellationToken cancellationToken)
        {
            var paginas = new List<PaginaParaRelatorioPaginacaoSoloDto>();

            var model = (RelatorioRegistroIndividualDto)request.Model;

            var nomeTemplateCabecalho = "RelatorioRegistroIndividualCabecalho";
            var nomeTemplateCorpo = "RelatorioRegistroIndividualCorpo";

            var htmlCabecalho = await htmlHelper.RenderRazorViewToString(nomeTemplateCabecalho, model.Cabecalho);
            htmlCabecalho = htmlCabecalho.Replace("logoMono.png", SmeConstants.LogoSmeMono);
            htmlCabecalho = htmlCabecalho.Replace("logo.png", SmeConstants.LogoSme);

            foreach (var aluno in model.Alunos)
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

            PdfGenerator pdfGenerator = new PdfGenerator(converter);
            pdfGenerator.ConvertToPdfPaginacaoSolo(paginas, nomeArquivo, request.CodigoCorrelacao.ToString());

            if (request.EnvioPorRabbit)
            {
                servicoFila.PublicaFila(new PublicaFilaDto(new MensagemRelatorioProntoDto(request.MensagemUsuario, request.MensagemTitulo), RotasRabbitSGP.RotaRelatoriosProntosSgp, ExchangeRabbit.Sgp, request.CodigoCorrelacao));
                return string.Empty;
            }
            else return request.CodigoCorrelacao.ToString();

        }
    }
}
