using DinkToPdf.Contracts;
using MediatR;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SME.SR.HtmlPdf;
using System.IO;
using SME.SR.Infra.Dtos;
using System.Linq;

namespace SME.SR.Application
{
    public class GerarRelatorioOcorrenciasCommandHandler : AsyncRequestHandler<GerarRelatorioOcorrenciasCommand>
    {
        private readonly IConverter converter;
        private readonly IHtmlHelper htmlHelper;
        private readonly IServicoFila servicoFila;

        public GerarRelatorioOcorrenciasCommandHandler(IConverter converter,
                                                       IHtmlHelper htmlHelper,
                                                       IServicoFila servicoFila)
        {
            this.converter = converter ?? throw new ArgumentNullException(nameof(converter));
            this.htmlHelper = htmlHelper ?? throw new ArgumentNullException(nameof(htmlHelper));
            this.servicoFila = servicoFila ?? throw new ArgumentNullException(nameof(servicoFila));
        }


        protected override async Task Handle(GerarRelatorioOcorrenciasCommand request, CancellationToken cancellationToken)
        {
            var paginas = new List<PaginaParaRelatorioPaginacaoSoloDto>();

            var limiteCaracteres = 5945;
            foreach (var ocorrencia in request.Relatorio.Ocorrencias)
            {
                var descricao = ocorrencia.DescricaoOcorrencia;
                var tamanhoDescricao = descricao.Length;
                var tamanhoDescricaoComAssinatura = tamanhoDescricao + 560;
                var numeroPaginasAluno = (tamanhoDescricaoComAssinatura / limiteCaracteres) + 1;

                for (int noPagina = 1; noPagina <= numeroPaginasAluno; noPagina++)
                {
                    var relatorio = new RelatorioRegistroOcorrenciasDto();
                    relatorio.DreNome = request.Relatorio.DreNome;
                    relatorio.UeNome = request.Relatorio.UeNome;
                    relatorio.Endereco = request.Relatorio.Endereco;
                    relatorio.Contato = request.Relatorio.Contato;
                    relatorio.UsuarioNome = request.Relatorio.UsuarioNome;
                    relatorio.UsuarioRF = request.Relatorio.UsuarioRF;

                    var descricaoOcorrencia = tamanhoDescricao <= limiteCaracteres ? descricao :
                        descricao.Substring(Math.Min((noPagina * limiteCaracteres) - limiteCaracteres, tamanhoDescricao),
                                            Math.Min(Math.Max(tamanhoDescricao - ((noPagina-1) * limiteCaracteres),0), limiteCaracteres));

                    ocorrencia.DescricaoOcorrencia = descricaoOcorrencia ?? string.Empty;
                    ocorrencia.ImprimirDadosOcorrencia = noPagina == 1;
                    ocorrencia.EhUltimaPagina = noPagina == numeroPaginasAluno;
                    relatorio.Ocorrencias = new List<RelatorioOcorrenciasDto>() { ocorrencia };

                    var html = await htmlHelper.RenderRazorViewToString("RelatorioRegistroOcorrencias", relatorio);
                    html = html.Replace("logoMono.png", SmeConstants.LogoSmeMono);
                    html = html.Replace("logo.png", SmeConstants.LogoSme);

                    paginas.Add(new PaginaParaRelatorioPaginacaoSoloDto(html,
                                                                        noPagina,
                                                                        numeroPaginasAluno));
                }
            }

            PdfGenerator pdfGenerator = new PdfGenerator(converter);

            var caminhoBase = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "relatorios");
            pdfGenerator.ConvertToPdfPaginacaoSolo(paginas, caminhoBase, request.CodigoCorrelacao.ToString(), "Relatório de ocorrências");

            await servicoFila.PublicaFila(new PublicaFilaDto(new MensagemRelatorioProntoDto(), RotasRabbitSGP.RotaRelatoriosProntosSgp, ExchangeRabbit.Sgp, request.CodigoCorrelacao));
        }
    }
}
