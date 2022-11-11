using System.Linq;
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

namespace SME.SR.Application 
{
    public class GerarRelatorioHtmlPDFPlanoAeeCommandHandler : AsyncRequestHandler<GerarRelatorioHtmlPDFPlanoAeeCommand>
    {
        private readonly IConverter converter;
        private readonly IHtmlHelper htmlHelper;
        private readonly IServicoFila servicoFila;
        private const int LINHAS_CABECALHO_PADRAO_LINHA_EM_BRANCO = 5;
        private const int LINHAS_PARECER_COORDENACAO_CEFAI = 3;
        private const int ADICIONAR_1_LINHA = 1;

        public GerarRelatorioHtmlPDFPlanoAeeCommandHandler(IConverter converter,
                                                       IHtmlHelper htmlHelper,
                                                       IServicoFila servicoFila)
        {
            this.converter = converter ?? throw new ArgumentNullException(nameof(converter));
            this.htmlHelper = htmlHelper ?? throw new ArgumentNullException(nameof(htmlHelper));
            this.servicoFila = servicoFila ?? throw new ArgumentNullException(nameof(servicoFila));
        }


        protected override async Task Handle(GerarRelatorioHtmlPDFPlanoAeeCommand request, CancellationToken cancellationToken)
        {
            var limiteCaracteres = 5700;
            var qtdeCaracteresPorLinha = 200;
            var qtdeCaracteresPagina = 0;
            var relatorioPlanoAeeDtos = new List<RelatorioPlanoAeeDto>();
            var paginasPlanoAee = new List<PaginaParaRelatorioPaginacaoSoloDto>();
            var relatorio = new RelatorioPlanoAeeDto();
            var listaQuestoes = new List<QuestaoPlanoAeeDto>();
            var qtdeCaracteresPaginaProposta = 0;

            foreach (var relatorioPlanoAeeDto in request.Relatorios)
            {
                relatorio = ObterRelatorioPlanoAeeDto(relatorioPlanoAeeDto);
                qtdeCaracteresPagina = ObterQtdeCaracteresPaginaCabecalho(qtdeCaracteresPorLinha);
                relatorio.Cadastro.Responsavel = relatorioPlanoAeeDto.Cadastro.Responsavel;
                qtdeCaracteresPagina += qtdeCaracteresPorLinha * ADICIONAR_1_LINHA;
                
                foreach (var questao in relatorioPlanoAeeDto.Cadastro.Questoes)
                {
                    var tmQuestao = TamanhoQuestaoRespostaJustificativa(questao, qtdeCaracteresPorLinha);
                    qtdeCaracteresPaginaProposta = qtdeCaracteresPagina + tmQuestao;

                    if (qtdeCaracteresPaginaProposta > limiteCaracteres)
                    {
                        relatorio.Cadastro.Questoes = listaQuestoes;
                        relatorioPlanoAeeDtos.Add(relatorio);
                        qtdeCaracteresPagina = ObterQtdeCaracteresPaginaCabecalho(qtdeCaracteresPorLinha);
                        relatorio = ObterRelatorioPlanoAeeDto(relatorioPlanoAeeDto);
                        listaQuestoes = new List<QuestaoPlanoAeeDto>(); 
                    }
                    qtdeCaracteresPagina += tmQuestao;
                    listaQuestoes.Add(questao);
                }
                
                relatorio.Cadastro.Questoes = listaQuestoes;
                
                qtdeCaracteresPagina += qtdeCaracteresPorLinha * LINHAS_PARECER_COORDENACAO_CEFAI;
                var tmParecer = TamanhoParecer(relatorioPlanoAeeDto.Parecer, qtdeCaracteresPorLinha);
                qtdeCaracteresPaginaProposta = qtdeCaracteresPagina + tmParecer;
                
                if (qtdeCaracteresPaginaProposta >= limiteCaracteres)
                {
                    relatorioPlanoAeeDtos.Add(relatorio);
                    relatorio = ObterRelatorioPlanoAeeDto(relatorioPlanoAeeDto);
                }
                relatorio.Parecer = relatorioPlanoAeeDto.Parecer;
                relatorioPlanoAeeDtos.Add(relatorio);

                var paginaPlanoAee = 0;
                var paginasTotais = relatorioPlanoAeeDtos.Count;
                
                foreach (var relatorioPlanoAee in relatorioPlanoAeeDtos)
                {
                    paginaPlanoAee++;
                    paginasPlanoAee.Add(await GerarPagina(paginasPlanoAee, relatorioPlanoAee, paginaPlanoAee,paginasTotais ));
                }

                relatorioPlanoAeeDtos = new List<RelatorioPlanoAeeDto>();                
            }

            var pdfGenerator = new PdfGenerator(converter);
            
            var caminhoBase = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "relatorios");
            pdfGenerator.ConvertToPdfPaginacaoSolo(paginasPlanoAee, caminhoBase, request.CodigoCorrelacao.ToString(), "Relatório do Plano Aee");
            
            await servicoFila.PublicaFila(new PublicaFilaDto(new MensagemRelatorioProntoDto(), RotasRabbitSGP.RotaRelatoriosProntosSgp, ExchangeRabbit.Sgp, request.CodigoCorrelacao));
        }

        private static RelatorioPlanoAeeDto ObterRelatorioPlanoAeeDto(RelatorioPlanoAeeDto relatorioPlanoAeeDto)
        {
            var relatorio = new RelatorioPlanoAeeDto() { Cabecalho = relatorioPlanoAeeDto.Cabecalho };
            return relatorio;
        }

        private int ObterQtdeCaracteresPaginaCabecalho(int qtdeCaracteresPorLinha)
        {
            int qtdeCaracteresPagina;
            qtdeCaracteresPagina = qtdeCaracteresPorLinha * LINHAS_CABECALHO_PADRAO_LINHA_EM_BRANCO;
            qtdeCaracteresPagina += qtdeCaracteresPorLinha * ADICIONAR_1_LINHA;
            return qtdeCaracteresPagina;
        }

        private int TamanhoQuestaoRespostaJustificativa(QuestaoPlanoAeeDto cadastro, int qtdeCaracteresPorLinha)
        {
            var qtdeCaracteres = CalcularQtdeLinhas(qtdeCaracteresPorLinha, cadastro.Questao.Length);

            if (!(cadastro.TipoQuestao == TipoQuestao.PeriodoEscolar || cadastro.TipoQuestao == TipoQuestao.FrequenciaEstudanteAEE))
                qtdeCaracteres += CalcularQtdeLinhas(qtdeCaracteresPorLinha, cadastro.Resposta.Length);

            if (cadastro.TipoQuestao == TipoQuestao.FrequenciaEstudanteAEE && cadastro.FrequenciaAluno.Any())
                qtdeCaracteres += cadastro.FrequenciaAluno.Count() + 1;

            if (!string.IsNullOrEmpty(cadastro.Justificativa))
            {
                var tamanhoJus = string.IsNullOrEmpty(cadastro.Justificativa) ? 0 : cadastro.Justificativa.Length;
                qtdeCaracteres += CalcularQtdeLinhas(qtdeCaracteresPorLinha, tamanhoJus);    
            }
            
            var tmBloco = qtdeCaracteres * qtdeCaracteresPorLinha;
            
            return (int)Math.Ceiling(tmBloco);
        }
        
        private int TamanhoParecer(ParecerPlanoAeeDto parecer, int qtdeCaracteresPorLinha)
        {
            var qtdeCaracteres = 0m;

            if (!string.IsNullOrEmpty(parecer.Coordenacao))
                qtdeCaracteres = CalcularQtdeLinhas(qtdeCaracteresPorLinha, parecer.Coordenacao.Length);    
            
            if (!string.IsNullOrEmpty(parecer.Cefai))
                qtdeCaracteres += CalcularQtdeLinhas(qtdeCaracteresPorLinha, parecer.Cefai.Length);

            if (!string.IsNullOrEmpty(parecer.PaaiResponsavel))
                qtdeCaracteres += CalcularQtdeLinhas(qtdeCaracteresPorLinha, parecer.PaaiResponsavel.Length);
            
            var tmBloco = qtdeCaracteres * qtdeCaracteresPorLinha;
            
            return (int)Math.Ceiling(tmBloco);
        }

        private decimal CalcularQtdeLinhas(int qtdeCaracteresPorLinha, int tamanhoTexto)
        {
            return Math.Ceiling(tamanhoTexto / Convert.ToDecimal(qtdeCaracteresPorLinha));
        }

        private async Task<PaginaParaRelatorioPaginacaoSoloDto> GerarPagina(List<PaginaParaRelatorioPaginacaoSoloDto> paginas, RelatorioPlanoAeeDto relatorio, int paginaPlanoAee, int totalPaginas)
        {
            var html = await htmlHelper.RenderRazorViewToString("RelatorioPlanoAee", relatorio);
            html = html.Replace("logoMono.png", SmeConstants.LogoSmeMono);
            html = html.Replace("logo.png", SmeConstants.LogoSme);

            return new PaginaParaRelatorioPaginacaoSoloDto(html, paginaPlanoAee, totalPaginas);
        }          
    }
}
