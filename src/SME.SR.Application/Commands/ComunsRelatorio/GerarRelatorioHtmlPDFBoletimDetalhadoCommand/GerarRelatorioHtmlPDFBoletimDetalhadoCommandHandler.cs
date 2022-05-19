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
    public class GerarRelatorioHtmlPDFBoletimDetalhadoCommandHandler : IRequestHandler<GerarRelatorioHtmlPDFBoletimDetalhadoCommand, string>
    {
        private readonly IServicoFila servicoFila;
        private readonly IHtmlHelper htmlHelper;
        private readonly IReportConverter reportConverter;

        public GerarRelatorioHtmlPDFBoletimDetalhadoCommandHandler(IConverter converter,
                                                              IServicoFila servicoFila,
                                                              IHtmlHelper htmlHelper, IReportConverter reportConverter)
        {
            this.servicoFila = servicoFila ?? throw new ArgumentNullException(nameof(servicoFila));
            this.htmlHelper = htmlHelper ?? throw new ArgumentNullException(nameof(htmlHelper));
            this.reportConverter = reportConverter ?? throw new ArgumentNullException(nameof(reportConverter));
        }

        public async Task<string> Handle(GerarRelatorioHtmlPDFBoletimDetalhadoCommand request, CancellationToken cancellationToken)
        {
            var paginas = new List<PaginaParaRelatorioPaginacaoSoloDto>();
            var caminhoBase = AppDomain.CurrentDomain.BaseDirectory;

            var model = (BoletimEscolarDetalhadoEscolaAquiDto)request.Model;

            const string nomeTemplateCabecalho = "RelatorioBoletimEscolarDetalhadoCabecalho";
            var nomeTemplateCorpo = request.Modalidade == Modalidade.EJA ? "RelatorioBoletimEscolarDetalhadoEJACorpo" : "RelatorioBoletimEscolarDetalhadoCorpo";

            foreach (var boletim in model.BoletimEscolarDetalhado.Boletins.OrderBy(a => a.Cabecalho.NomeTurma))
            {
                var htmlCabecalho = await htmlHelper.RenderRazorViewToString(nomeTemplateCabecalho, boletim.Cabecalho);
                htmlCabecalho = htmlCabecalho.Replace("logoMono.png", SmeConstants.LogoSmeMono);
                htmlCabecalho = htmlCabecalho.Replace("logo.png", SmeConstants.LogoSme);
                htmlCabecalho = htmlCabecalho.Replace("#PASTACSS", Path.Combine(caminhoBase, "assets/css"));
                htmlCabecalho = htmlCabecalho.Replace("#PASTAFONTS", Path.Combine(caminhoBase, "assets/fonts"));

                var htmlCorpo = await htmlHelper.RenderRazorViewToString(nomeTemplateCorpo, boletim);

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

            var nomeArquivo = Path.Combine(caminhoBase, "relatorios");

            reportConverter.ConvertToPdfPaginacaoSolo(paginas, nomeArquivo, request.CodigoCorrelacao.ToString(), DateTime.Now.ToString("dd/MM/yyyy"));

            if (request.EnvioPorRabbit)
            {
                var filaRabbit = !string.IsNullOrEmpty(request.MensagemDados) ? RotasRabbitSGP.RotaRelatoriosProntosApp : RotasRabbitSGP.RotaRelatoriosProntosSgp;
                await servicoFila.PublicaFila(new PublicaFilaDto(new MensagemRelatorioProntoDto(request.MensagemUsuario, request.MensagemTitulo, request.MensagemDados), filaRabbit, ExchangeRabbit.Sgp, request.CodigoCorrelacao));
                return string.Empty;
            }

            return request.CodigoCorrelacao.ToString();
        }
    }
}
