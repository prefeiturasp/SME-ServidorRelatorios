using MediatR;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using SME.SR.Application.Interfaces;
using SME.SR.Infra;
using SME.SR.Infra.EncaminhamentoNaapa;
using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioEncaminhamentosNAAPADetalhadoUseCase : IRelatorioEncaminhamentosNaapaDetalhadoUseCase
    {
        private readonly IMediator mediator;
        private const string NOME_COMPONENTE_SECAO_INFORMACOES_ESTUDANTE = "INFORMACOES_ESTUDANTE";
        private const string NOME_COMPONENTE_SECAO_QUESTOES_APRESENTADAS_INFANTIL = "QUESTOES_APRESENTADAS_INFANTIL";
        private const string NOME_COMPONENTE_SECAO_QUESTOES_APRESENTADAS_FUNDAMENTAL = "QUESTOES_APRESENTADAS_FUNDAMENTAL";
        private const string NOME_COMPONENTE_SECAO_QUESTOES_ITINERANCIA = "QUESTOES_ITINERACIA";

        private const string NOME_SECAO_INFORMACOES_ESTUDANTE = "INFORMAÇÕES";
        private const string NOME_SECAO_QUESTOES_APRESENTADAS_INFANTIL = "QUESTÕES APRESENTADAS";
        private const string NOME_SECAO_QUESTOES_APRESENTADAS_FUNDAMENTAL = "QUESTÕES APRESENTADAS";
        private const string NOME_SECAO_QUESTOES_ITINERANCIA = "ITINERÂNCIA";

        public RelatorioEncaminhamentosNAAPADetalhadoUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto request)
        {
            var filtroRelatorio = request.ObterObjetoFiltro<FiltroRelatorioEncaminhamentoNAAPADetalhadoDto>(); 
            var encaminhamentosNaapa = await mediator.Send(new ObterEncaminhamentosNAAPAQuery(filtroRelatorio));

            if (encaminhamentosNaapa == null || !encaminhamentosNaapa.Any())
                throw new NegocioException("Nenhuma informação para os filtros informados.");

            var relatorios = new List<RelatorioEncaminhamentoNAAPADetalhadoDto>();

            foreach (var encaminhamentoNaapa in encaminhamentosNaapa)
            {
                var relatorio = new RelatorioEncaminhamentoNAAPADetalhadoDto();

                await ObterCabecalho(encaminhamentoNaapa, relatorio);
                await ObterQuestoesSecoes(encaminhamentoNaapa, relatorio);

                relatorios.Add(relatorio);
            }

            await mediator.Send(new GerarRelatorioHtmlPDFEncaminhamentoNaapaDetalhadoCommand(relatorios, request.CodigoCorrelacao));
        }

        private async Task ObterCabecalho(EncaminhamentoNAAPADto encaminhamentoNaapa, RelatorioEncaminhamentoNAAPADetalhadoDto relatorio)
        {
            relatorio.Cabecalho.DreNome = encaminhamentoNaapa.DreAbreviacao;
            relatorio.Cabecalho.UeNome = $"{encaminhamentoNaapa.UeCodigo} - {encaminhamentoNaapa.TipoEscola.ShortName()} {encaminhamentoNaapa.UeNome}";
            relatorio.Cabecalho.AnoLetivo = encaminhamentoNaapa.AnoLetivo;
            relatorio.Cabecalho.Aluno = $"{encaminhamentoNaapa.AlunoNome} ({encaminhamentoNaapa.AlunoCodigo})";
            relatorio.Cabecalho.SituacaoEncaminhamento = ((SituacaoNAAPA)encaminhamentoNaapa.Situacao).Name();
            relatorio.Cabecalho.TurmaNome = $"{encaminhamentoNaapa.Modalidade.ShortName()} - {encaminhamentoNaapa.TurmaNome}";
            relatorio.Cabecalho.DataCriacao = encaminhamentoNaapa.CriadoEm.ToString("dd/MM/yyyy");
            relatorio.Cabecalho.DataImpressao = DateTime.Now.ToString("dd/MM/yyyy");

            var dadosAluno = await mediator.Send(new ObterDadosAlunoQuery { CodigoAluno = encaminhamentoNaapa.AlunoCodigo, CodigoTurma = encaminhamentoNaapa.TurmaCodigo });
            relatorio.Cabecalho.DataNascimento = dadosAluno.DataNascimento.ToString("dd/MM/yyyy");
        }

        private async Task ObterQuestoesSecoes(EncaminhamentoNAAPADto encaminhamentoNaapa, RelatorioEncaminhamentoNAAPADetalhadoDto relatorio)
        {
            var questoes = await mediator.Send(new ObterQuestoesEncaminhamentoNAAPAPorIdENomeComponenteSecaoQuery(encaminhamentoNaapa.Id, NOME_COMPONENTE_SECAO_INFORMACOES_ESTUDANTE));
            relatorio.Detalhes.Informacoes = await ObterSecaoQuestoesQuestionario(encaminhamentoNaapa, questoes, NOME_COMPONENTE_SECAO_INFORMACOES_ESTUDANTE, NOME_SECAO_INFORMACOES_ESTUDANTE);

            if (encaminhamentoNaapa.Modalidade == Modalidade.Infantil)
            {
                questoes = await mediator.Send(new ObterQuestoesEncaminhamentoNAAPAPorIdENomeComponenteSecaoQuery(encaminhamentoNaapa.Id, NOME_COMPONENTE_SECAO_QUESTOES_APRESENTADAS_INFANTIL));
                relatorio.Detalhes.QuestoesApresentadas = await ObterSecaoQuestoesQuestionario(encaminhamentoNaapa, questoes, NOME_COMPONENTE_SECAO_QUESTOES_APRESENTADAS_INFANTIL, NOME_SECAO_QUESTOES_APRESENTADAS_INFANTIL);
            }
            else
            {
                questoes = await mediator.Send(new ObterQuestoesEncaminhamentoNAAPAPorIdENomeComponenteSecaoQuery(encaminhamentoNaapa.Id, NOME_COMPONENTE_SECAO_QUESTOES_APRESENTADAS_FUNDAMENTAL));
                relatorio.Detalhes.QuestoesApresentadas = await ObterSecaoQuestoesQuestionario(encaminhamentoNaapa, questoes, NOME_COMPONENTE_SECAO_QUESTOES_APRESENTADAS_FUNDAMENTAL, NOME_SECAO_QUESTOES_APRESENTADAS_FUNDAMENTAL);
            }

            var secoes = await mediator.Send(new ObterSecoesEncaminhamentoNAAPAPorIdENomeComponenteSecaoQuery(encaminhamentoNaapa.Id, NOME_COMPONENTE_SECAO_QUESTOES_ITINERANCIA));
            relatorio.Detalhes.Itinerancia = await ObterSecaoQuestoesQuestionario(encaminhamentoNaapa, secoes, NOME_COMPONENTE_SECAO_QUESTOES_ITINERANCIA, NOME_SECAO_QUESTOES_ITINERANCIA);
        }

        private async Task<IEnumerable<SecaoQuestoesEncaminhamentoNAAPAItineranciaDetalhadoDto>> ObterSecaoQuestoesQuestionario(EncaminhamentoNAAPADto encaminhamentoNaapa, IEnumerable<SecaoEncaminhamentoNAAPADto> secoes, string nomeComponenteSecao, string nomeSecao)
        {
            var itinerancias = new List<SecaoQuestoesEncaminhamentoNAAPAItineranciaDetalhadoDto>();
            
            if (secoes != null)
            {
                foreach (var secao in secoes)
                {
                    var secaoDetalhado = new SecaoQuestoesEncaminhamentoNAAPAItineranciaDetalhadoDto(nomeComponenteSecao, nomeSecao, secao.SecaoId, secao.DataAtendimento, secao.TipoAtendimento, secao.CriadoPor);
                    await AdicionarQuestoesQuestionarioSecao(encaminhamentoNaapa, secaoDetalhado, secao.Questoes);
                    itinerancias.Add(secaoDetalhado);
                }
            }

            return itinerancias;
        }

        private async Task<SecaoQuestoesEncaminhamentoNAAPADetalhadoDto> ObterSecaoQuestoesQuestionario(EncaminhamentoNAAPADto encaminhamentoNaapa, IEnumerable<QuestaoDto> questoes, string nomeComponenteSecao, string nomeSecao)
        {
            var secao = new SecaoQuestoesEncaminhamentoNAAPADetalhadoDto(nomeComponenteSecao, nomeSecao);
            await AdicionarQuestoesQuestionarioSecao(encaminhamentoNaapa, secao, questoes);
            return secao;
        }

        private async Task AdicionarQuestoesQuestionarioSecao(EncaminhamentoNAAPADto encaminhamentoNaapa, SecaoQuestoesEncaminhamentoNAAPADetalhadoDto secao, IEnumerable<QuestaoDto> questoes, string OrdemPai = "")
        {
            foreach (var questao in questoes)
            {
                if (questao.Tipo == TipoQuestao.Upload || questao.Respostas == null || !questao.Respostas.Any())
                    continue;

                var questaoRelatorio = new QuestaoEncaminhamentoNAAPADetalhadoDto
                {
                    Questao = questao.Nome,
                    Ordem = questao.Ordem,
                    OrdemMascara = $"{(string.IsNullOrEmpty(OrdemPai) ? string.Empty : $"{ OrdemPai }.")}{questao.Ordem}",
                    QuestaoId = questao.Id,
                    TipoQuestao = questao.Tipo,
                    NomeComponente = questao.NomeComponente
                };

                if (questao.Tipo != TipoQuestao.Radio &&
                    questao.Tipo != TipoQuestao.Combo &&
                    questao.Tipo != TipoQuestao.ComboMultiplaEscolha &&
                    questao.Tipo != TipoQuestao.Checkbox)
                {
                    var resposta = questao.Respostas?.FirstOrDefault();
                    questaoRelatorio.Resposta = resposta?.Texto;

                    if (resposta != null)
                    {
                        switch (questaoRelatorio.TipoQuestao)
                        {
                            case TipoQuestao.Endereco:
                                questaoRelatorio.Enderecos = ObterRespostaParaObjeto<EnderecoDto>(resposta);
                                break;
                            case TipoQuestao.ContatoResponsavel:
                                questaoRelatorio.ContatoResponsaveis = ObterRespostaParaObjeto<ContatoResponsaveisDto>(resposta);
                                break;
                            case TipoQuestao.AtividadesContraturno:
                                questaoRelatorio.AtividadeContraTurnos = ObterRespostaParaObjeto<AtividadeContraTurnoDto>(resposta);
                                break;

                            case TipoQuestao.TurmasPrograma:
                                questaoRelatorio.TurmasPrograma = ObterRespostaParaObjeto<TurmaProgramaDto>(resposta);
                                break;
                        }
                    }
                }
                else
                    foreach (var resposta in questao.Respostas)
                    {
                        var respostaOpcao = resposta.Texto;
                        var opcaoRespostaQuestao = questao.OpcaoResposta.FirstOrDefault(c => c.QuestaoId == questao.Id &&
                                                    c.Id == resposta.OpcaoRespostaId);

                        if (opcaoRespostaQuestao != null)
                            respostaOpcao = opcaoRespostaQuestao.Nome;

                        questaoRelatorio.Resposta += $"{(!string.IsNullOrEmpty(questaoRelatorio.Resposta) ? " | " : string.Empty)}{respostaOpcao}";
                    }

                if (!secao.Questoes.Any(questao => questao.QuestaoId == questaoRelatorio.QuestaoId || questao.OrdemMascara == questaoRelatorio.OrdemMascara))
                    secao.Questoes.Add(questaoRelatorio);

                if (questao.Tipo == TipoQuestao.Radio ||
                    questao.Tipo == TipoQuestao.Combo ||
                    questao.Tipo == TipoQuestao.ComboMultiplaEscolha ||
                    questao.Tipo == TipoQuestao.Checkbox)
                {
                    foreach (var resposta in questao.Respostas)
                    {
                        var opcaoRespostaQuestao = questao.OpcaoResposta.FirstOrDefault(c => c.QuestaoId == questao.Id &&
                                                    c.Id == resposta.OpcaoRespostaId);

                        if (opcaoRespostaQuestao != null && opcaoRespostaQuestao.QuestoesComplementares != null && opcaoRespostaQuestao.QuestoesComplementares.Any())
                            await AdicionarQuestoesQuestionarioSecao(encaminhamentoNaapa, secao,
                                                                     opcaoRespostaQuestao.QuestoesComplementares, questao.Ordem.ToString());

                    }

                }
            }
        }

        private IEnumerable<T> ObterRespostaParaObjeto<T>(RespostaQuestaoDto respostaQuestao)
        {
            if (string.IsNullOrEmpty(respostaQuestao.Texto))
                return null;

            var resposta = JsonConvert.DeserializeObject<List<T>>(respostaQuestao.Texto);
            return resposta;
        }
    }
}