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
        private List<PaginaParaRelatorioPaginacaoSoloDto> paginas;

        public GerarRelatorioHtmlPDFBoletimDetalhadoCommandHandler(IConverter converter,
                                                              IServicoFila servicoFila,
                                                              IHtmlHelper htmlHelper, IReportConverter reportConverter)
        {
            this.servicoFila = servicoFila ?? throw new ArgumentNullException(nameof(servicoFila));
            this.htmlHelper = htmlHelper ?? throw new ArgumentNullException(nameof(htmlHelper));
            this.reportConverter = reportConverter ?? throw new ArgumentNullException(nameof(reportConverter));
            this.paginas = new List<PaginaParaRelatorioPaginacaoSoloDto>();
        }

        public async Task<string> Handle(GerarRelatorioHtmlPDFBoletimDetalhadoCommand request, CancellationToken cancellationToken)
        {
            var caminhoBase = AppDomain.CurrentDomain.BaseDirectory;
            var model = (BoletimEscolarDetalhadoEscolaAquiDto)request.Model;

            if (model.BoletimEscolarDetalhado.ExibirRecomendacao)
            {
                await CarreguePaginasComExibicaoDeRecomendacao(model.BoletimEscolarDetalhado, request.Modalidade);
            }
            else
            {
                await CarreguePaginasSemExibicaoDeRecomendacao(model.BoletimEscolarDetalhado, request.Modalidade);
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

        private string ObterHtmlComLogo(string html)
        {
            return html.Replace("logoMono.png", SmeConstants.LogoSmeMono)
                       .Replace("logo.png", SmeConstants.LogoSme)
                       .Replace("#PASTACSS", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "assets/css"))
                       .Replace("#PASTAFONTS", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "assets/fonts"));
        }

        private async Task CarreguePaginasComExibicaoDeRecomendacao(BoletimEscolarDetalhadoDto boletimDto, Modalidade modalidade)
        {
            const string nomeTemplateCabecalho = "RelatorioBoletimEscolarDetalhadoCabecalho";
            var nomeTemplateCorpo = modalidade.EhSemestral() ? "RelatorioBoletimEscolarDetalhadoEJACorpo" : "RelatorioBoletimEscolarDetalhadoCorpo";

            foreach (var boletim in boletimDto.Boletins.OrderBy(a => a.Cabecalho.NomeTurma))
            {
                var htmlCabecalho = await htmlHelper.RenderRazorViewToString(nomeTemplateCabecalho, boletim.Cabecalho);
                htmlCabecalho = ObterHtmlComLogo(htmlCabecalho);

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
        }

        private async Task CarreguePaginasSemExibicaoDeRecomendacao(BoletimEscolarDetalhadoDto boletimDto, Modalidade modalidade)
        {
            var boletinsPorPagina = ObterBoletinsPorPagina(boletimDto, modalidade);
            var iNumPagina = 1;

            foreach (var pagina in boletinsPorPagina)
            {
                var html = await htmlHelper.RenderRazorViewToString("RelatorioBoletimEscolarDetalhadoLista", pagina);

                html = ObterHtmlComLogo(html);
                paginas.Add(new PaginaParaRelatorioPaginacaoSoloDto(html, iNumPagina, boletinsPorPagina.Count));
                iNumPagina++;
            }
        }

        private  List<BoletimEscolarDetalhadoDto> ObterBoletinsPorPagina(BoletimEscolarDetalhadoDto boletimDto, Modalidade modalidade)
        {
            return boletimDto.Boletins.Select((boletim, i) => new { Index = i, Value = boletim })
                .GroupBy(boletim => boletim.Index / 2)
                .Select(boletim => new BoletimEscolarDetalhadoDto { Boletins = boletim.Select(v => v.Value).ToList(), Modalidade = modalidade })
                .ToList();
        }
    }
}
