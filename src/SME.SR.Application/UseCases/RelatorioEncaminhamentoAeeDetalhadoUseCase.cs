using MediatR;
using Newtonsoft.Json;
using Org.BouncyCastle.Ocsp;
using SME.SR.Application.Interfaces;
using SME.SR.Data;
using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioEncaminhamentoAeeDetalhadoUseCase : IRelatorioEncaminhamentoAeeDetalhadoUseCase
    {
        private readonly IMediator mediator;
        private const string SECAO_INFORMACOES_ESCOLARES = "INFORMACOES_ESCOLARES";
        private const string SECAO_DESCRICAO_ENCAMINHAMENTO = "DESCRICAO_ENCAMINHAMENTO";
        private const string SECAO_PARECER_COORDENACAO = "PARECER_COORDENACAO";
        private const string SECAO_PARECER_AEE = "PARECER_AEE";


        public RelatorioEncaminhamentoAeeDetalhadoUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto request)
        {
            var filtroRelatorio = request.ObterObjetoFiltro<FiltroRelatorioEncaminhamentoAeeDetalhadoDto>();
            var encaminhamentosAee = await mediator.Send(new ObterEncaminhamentosAEEPorIdQuery(filtroRelatorio));
            
            if (encaminhamentosAee == null || !encaminhamentosAee.Any())
                throw new NegocioException("Nenhuma informação para os filtros informados.");

            var relatorios = new List<RelatorioEncaminhamentoAeeDetalhadoDto>();

            foreach (var encaminhamentoAee in encaminhamentosAee)
            {
                var relatorio = new RelatorioEncaminhamentoAeeDetalhadoDto();

                ObterCabecalho(encaminhamentoAee, relatorio);
                await ObterQuestoesSecoes(encaminhamentoAee, relatorio);

                relatorios.Add(relatorio);
            }

            await mediator.Send(new GerarRelatorioHtmlPDFEncaminhamentoAeeDetalhadoCommand(relatorios,  request.CodigoCorrelacao));
        }

        private static void ObterCabecalho(EncaminhamentoAeeDto encaminhamentoAee, RelatorioEncaminhamentoAeeDetalhadoDto relatorioEncaminhamentoAeeDetalhado)
        {
            relatorioEncaminhamentoAeeDetalhado.Cabecalho.DreNome = encaminhamentoAee.DreAbreviacao;
            relatorioEncaminhamentoAeeDetalhado.Cabecalho.UeNome = $"{encaminhamentoAee.UeCodigo} - {encaminhamentoAee.TipoEscola.ShortName()} {encaminhamentoAee.UeNome}";
            relatorioEncaminhamentoAeeDetalhado.Cabecalho.AnoLetivo = encaminhamentoAee.AnoLetivo;
            relatorioEncaminhamentoAeeDetalhado.Cabecalho.AlunoNome = encaminhamentoAee.AlunoNome;
            relatorioEncaminhamentoAeeDetalhado.Cabecalho.SituacaoEncaminhamento = ((SituacaoEncaminhamentoAEE) encaminhamentoAee.Situacao).Name();
            relatorioEncaminhamentoAeeDetalhado.Cabecalho.TurmaNome = $"{encaminhamentoAee.Modalidade.ShortName()} - {encaminhamentoAee.TurmaNome}";
            relatorioEncaminhamentoAeeDetalhado.Cabecalho.AlunoCodigo = encaminhamentoAee.AlunoCodigo;
            relatorioEncaminhamentoAeeDetalhado.Cabecalho.DataCriacao = encaminhamentoAee.CriadoEm;
        }

        private async Task ObterQuestoesSecoes(EncaminhamentoAeeDto encaminhamentoAee, RelatorioEncaminhamentoAeeDetalhadoDto relatorioEncaminhamentoAeeDetalhado)
        {
            var questoes = await mediator.Send(new ObterQuestoesEncaminhamentoAEEPorIdENomeComponenteSecaoQuery(encaminhamentoAee.Id, SECAO_INFORMACOES_ESCOLARES));
            relatorioEncaminhamentoAeeDetalhado.Detalhes.InformacoesEscolares = await ObterQuestoesQuestionario(questoes.ToList(), SECAO_INFORMACOES_ESCOLARES);

            questoes = await mediator.Send(new ObterQuestoesEncaminhamentoAEEPorIdENomeComponenteSecaoQuery(encaminhamentoAee.Id, SECAO_DESCRICAO_ENCAMINHAMENTO));
            relatorioEncaminhamentoAeeDetalhado.Detalhes.DescricaoEncaminhamento = await ObterQuestoesQuestionario(questoes.ToList(), SECAO_DESCRICAO_ENCAMINHAMENTO);

            questoes = await mediator.Send(new ObterQuestoesEncaminhamentoAEEPorIdENomeComponenteSecaoQuery(encaminhamentoAee.Id, SECAO_PARECER_COORDENACAO));
            relatorioEncaminhamentoAeeDetalhado.Detalhes.ParecerCoordenacao = await ObterQuestoesQuestionario(questoes.ToList(), SECAO_PARECER_COORDENACAO);

            questoes = await mediator.Send(new ObterQuestoesEncaminhamentoAEEPorIdENomeComponenteSecaoQuery(encaminhamentoAee.Id, SECAO_PARECER_AEE));
            relatorioEncaminhamentoAeeDetalhado.Detalhes.ParecerAee = await ObterQuestoesQuestionario(questoes.ToList(), SECAO_PARECER_AEE);
        }

        private async Task<SecaoQuestoesEncaminhamentoAeeDto> ObterQuestoesQuestionario(List<QuestaoDto> questoes, string nomeComponenteSecao)
        {
            var secao = new SecaoQuestoesEncaminhamentoAeeDto() { NomeComponenteSecao = nomeComponenteSecao };

            foreach (var questao in questoes)
            {
                var questaoRelatorio = new QuestaoEncaminhamentoAeeDto
                {
                    Questao = questao.Nome,
                    Ordem = questao.Ordem,
                    QuestaoId = questao.Id,
                    TipoQuestao = questao.Tipo
                };

                if (questao.Respostas == null || !questao.Respostas.Any())
                    continue;

                foreach (var resposta in questao.Respostas)
                {
                    var respostaEncaminhamento = new RespostaEncaminhamentoAeeDto()
                                                            {
                                                                RespostaId = resposta.OpcaoRespostaId,
                                                                Resposta = resposta.Texto,
                                                            };

                    var opcaoRespostaQuestao = questao.OpcaoResposta.FirstOrDefault(c => c.QuestaoId == questao.Id &&
                                                c.Id == resposta.OpcaoRespostaId);

                    if (opcaoRespostaQuestao != null) {
                        if (questao.Tipo == TipoQuestao.Radio || questao.Tipo == TipoQuestao.ComboMultiplaEscolha)
                            respostaEncaminhamento.Resposta = opcaoRespostaQuestao.Nome;
                        respostaEncaminhamento.Justificativa = ObterJustificativaQuestao(opcaoRespostaQuestao);

                        var atendimentoClinico = ObterAtencimentoClinicoQuestao(opcaoRespostaQuestao);
                        if (atendimentoClinico!=null)
                        {
                            var respostaAtendimentoClinicoAluno = ObterRespostaAtendimentoClinicoAluno(atendimentoClinico);
                            if (respostaAtendimentoClinicoAluno != null)
                                secao.AtendimentoClinico = respostaAtendimentoClinicoAluno;
                        }
                    }
                    questaoRelatorio.Respostas.Add(respostaEncaminhamento);
                }

                if (questao.Tipo == TipoQuestao.InformacoesEscolares)
                {
                    var respostaInformacaoEscolarAluno = ObterRespostaInformacaoEscolarAluno(questao.Respostas.FirstOrDefault());
                    if (respostaInformacaoEscolarAluno != null)
                        secao.InformacaoEscolar = respostaInformacaoEscolarAluno;
                }

                secao.Questoes.Add(questaoRelatorio);
            }

            return secao;
        }

        private static List<AtendimentoClinicoAlunoDto> ObterRespostaAtendimentoClinicoAluno(RespostaQuestaoDto respostaQuestao)
        {
            if (string.IsNullOrEmpty(respostaQuestao.Texto))
                return null;
            var resposta = JsonConvert.DeserializeObject<List<AtendimentoClinicoAlunoDto>>(respostaQuestao.Texto);
            return resposta;
        }

        private static List<InformacaoEscolarAlunoDto> ObterRespostaInformacaoEscolarAluno(RespostaQuestaoDto respostaQuestao)
        {
            if (string.IsNullOrEmpty(respostaQuestao.Texto))
                return null;
            var resposta = JsonConvert.DeserializeObject<List<InformacaoEscolarAlunoDto>>(respostaQuestao.Texto);
            return resposta;
        }

        private static string ObterJustificativaQuestao(OpcaoRespostaDto opcaoResposta)
        {
            if (opcaoResposta?.QuestoesComplementares == null)
                return string.Empty;

            var questaoComplementar = opcaoResposta.QuestoesComplementares.FirstOrDefault();

            var respostaQuestaoComplementar =
                questaoComplementar?.Respostas.FirstOrDefault(c => c.QuestaoId == questaoComplementar.Id);

            return UtilRegex.RemoverTagsHtml(respostaQuestaoComplementar?.Texto);
        }

        private static RespostaQuestaoDto ObterAtencimentoClinicoQuestao(OpcaoRespostaDto opcaoResposta)
        {
            if (opcaoResposta?.QuestoesComplementares == null)
                return null;

            var questaoComplementar = opcaoResposta.QuestoesComplementares.FirstOrDefault();
            if (questaoComplementar.Tipo != TipoQuestao.AtendimentoClinico)
                return null;

            var respostaQuestaoComplementar =
                questaoComplementar?.Respostas.FirstOrDefault(c => c.QuestaoId == questaoComplementar.Id);

            return respostaQuestaoComplementar;
        }
    }
}
