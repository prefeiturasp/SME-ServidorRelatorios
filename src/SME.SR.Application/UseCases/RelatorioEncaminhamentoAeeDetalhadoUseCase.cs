using DocumentFormat.OpenXml.Spreadsheet;
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
using System.Text;
using System.Threading.Tasks;
using SME.SR.Application.Commands.ComunsRelatorio.GerarRelatorioHtmlParaPdf;

namespace SME.SR.Application
{
    public class RelatorioEncaminhamentoAeeDetalhadoUseCase : IRelatorioEncaminhamentoAeeDetalhadoUseCase
    {
        private readonly IMediator mediator;
        private const string NOME_COMPONENTE_SECAO_INFORMACOES_ESCOLARES = "INFORMACOES_ESCOLARES";
        private const string NOME_COMPONENTE_SECAO_DESCRICAO_ENCAMINHAMENTO = "DESCRICAO_ENCAMINHAMENTO";
        private const string NOME_COMPONENTE_SECAO_PARECER_COORDENACAO = "PARECER_COORDENACAO";
        private const string NOME_COMPONENTE_SECAO_PARECER_AEE = "PARECER_AEE";


        private const string NOME_SECAO_INFORMACOES_ESCOLARES = "INFORMAÇÕES ESCOLARES";
        private const string NOME_SECAO_DESCRICAO_ENCAMINHAMENTO = "DESCRIÇÃO DO ENCAMINHAMENTO";
        private const string NOME_SECAO_PARECER_COORDENACAO = "PARECER DA COORDENAÇÃO";
        private const string NOME_SECAO_PARECER_AEE = "PARECER AEE";

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

            await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioEncaminhamentoAEEDetalhado", relatorios, request.CodigoCorrelacao, tipoDePaginas:EnumTipoDePaginas.PaginaSemTotalPaginas));
        }

        private static void ObterCabecalho(EncaminhamentoAeeDto encaminhamentoAee, RelatorioEncaminhamentoAeeDetalhadoDto relatorioEncaminhamentoAeeDetalhado)
        {
            relatorioEncaminhamentoAeeDetalhado.Cabecalho.DreNome = encaminhamentoAee.DreAbreviacao;
            relatorioEncaminhamentoAeeDetalhado.Cabecalho.UeNome = $"{encaminhamentoAee.UeCodigo} - {encaminhamentoAee.TipoEscola.ShortName()} {encaminhamentoAee.UeNome}";
            relatorioEncaminhamentoAeeDetalhado.Cabecalho.AnoLetivo = encaminhamentoAee.AnoLetivo;
            relatorioEncaminhamentoAeeDetalhado.Cabecalho.Aluno = $"{encaminhamentoAee.AlunoNome} ({encaminhamentoAee.AlunoCodigo})";
            relatorioEncaminhamentoAeeDetalhado.Cabecalho.SituacaoEncaminhamento = ((SituacaoEncaminhamentoAEE) encaminhamentoAee.Situacao).Name();
            relatorioEncaminhamentoAeeDetalhado.Cabecalho.TurmaNome = $"{encaminhamentoAee.Modalidade.ShortName()} - {encaminhamentoAee.TurmaNome}";
            relatorioEncaminhamentoAeeDetalhado.Cabecalho.ResponsavelPaai = !string.IsNullOrEmpty(encaminhamentoAee.ResponsavelPaaiNome) ? $"{encaminhamentoAee.ResponsavelPaaiNome} ({encaminhamentoAee.ResponsavelPaaiLoginRf})" : string.Empty;
            relatorioEncaminhamentoAeeDetalhado.Cabecalho.DataCriacao = encaminhamentoAee.CriadoEm;
        }

        private async Task ObterQuestoesSecoes(EncaminhamentoAeeDto encaminhamentoAee, RelatorioEncaminhamentoAeeDetalhadoDto relatorioEncaminhamentoAeeDetalhado)
        {
            var questoes = await mediator.Send(new ObterQuestoesEncaminhamentoAEEPorIdENomeComponenteSecaoQuery(encaminhamentoAee.Id, NOME_COMPONENTE_SECAO_INFORMACOES_ESCOLARES));
            relatorioEncaminhamentoAeeDetalhado.Detalhes.InformacoesEscolares = await ObterSecaoQuestoesQuestionario(encaminhamentoAee, questoes.ToList(), NOME_COMPONENTE_SECAO_INFORMACOES_ESCOLARES, NOME_SECAO_INFORMACOES_ESCOLARES);

            questoes = await mediator.Send(new ObterQuestoesEncaminhamentoAEEPorIdENomeComponenteSecaoQuery(encaminhamentoAee.Id, NOME_COMPONENTE_SECAO_DESCRICAO_ENCAMINHAMENTO));
            relatorioEncaminhamentoAeeDetalhado.Detalhes.DescricaoEncaminhamento = await ObterSecaoQuestoesQuestionario(encaminhamentoAee, questoes.ToList(), NOME_COMPONENTE_SECAO_DESCRICAO_ENCAMINHAMENTO, NOME_SECAO_DESCRICAO_ENCAMINHAMENTO);

            questoes = await mediator.Send(new ObterQuestoesEncaminhamentoAEEPorIdENomeComponenteSecaoQuery(encaminhamentoAee.Id, NOME_COMPONENTE_SECAO_PARECER_COORDENACAO));
            relatorioEncaminhamentoAeeDetalhado.Detalhes.ParecerCoordenacao = await ObterSecaoQuestoesQuestionario(encaminhamentoAee, questoes.ToList(), NOME_COMPONENTE_SECAO_PARECER_COORDENACAO, NOME_SECAO_PARECER_COORDENACAO);

            questoes = await mediator.Send(new ObterQuestoesEncaminhamentoAEEPorIdENomeComponenteSecaoQuery(encaminhamentoAee.Id, NOME_COMPONENTE_SECAO_PARECER_AEE));
            relatorioEncaminhamentoAeeDetalhado.Detalhes.ParecerAee = await ObterSecaoQuestoesQuestionario(encaminhamentoAee, questoes.ToList(), NOME_COMPONENTE_SECAO_PARECER_AEE, NOME_SECAO_PARECER_AEE);
        }

        private async Task<SecaoQuestoesEncaminhamentoAeeDto> ObterSecaoQuestoesQuestionario(EncaminhamentoAeeDto encaminhamentoAee, List<QuestaoDto> questoes, string nomeComponenteSecao, string nomeSecao)
        {
            var secao = new SecaoQuestoesEncaminhamentoAeeDto() { NomeComponenteSecao = nomeComponenteSecao, NomeSecao = nomeSecao };
            await AdicionarQuestoesQuestionarioSecao(encaminhamentoAee, secao, questoes);
            return secao;
        }

        private async Task AdicionarQuestoesQuestionarioSecao(EncaminhamentoAeeDto encaminhamentoAee, SecaoQuestoesEncaminhamentoAeeDto secao, List<QuestaoDto> questoes, string OrdemPai = "")
        {

            foreach (var questao in questoes)
            {
                if (questao.Tipo == TipoQuestao.Upload || questao.Respostas == null || !questao.Respostas.Any())
                    continue;

                var questaoRelatorio = new QuestaoEncaminhamentoAeeDto
                {
                    Questao = questao.Nome,
                    Ordem = questao.Ordem,
                    OrdemMascara = $"{(string.IsNullOrEmpty(OrdemPai) ? string.Empty : $"{ OrdemPai }.")}{questao.Ordem}",
                    QuestaoId = questao.Id,
                    TipoQuestao = questao.Tipo
                };

                if (questao.Tipo != TipoQuestao.Radio &&
                    questao.Tipo != TipoQuestao.ComboMultiplaEscolha &&
                    questao.Tipo != TipoQuestao.Checkbox)
                {
                    var resposta = questao.Respostas?.FirstOrDefault();
                    questaoRelatorio.Resposta = resposta?.Texto;

                    if (resposta != null)
                    {
                        if (questaoRelatorio.TipoQuestao == TipoQuestao.AtendimentoClinico)
                            questaoRelatorio.AtendimentoClinico = ObterAtendimentosClinicosAluno(resposta);
                        if (questaoRelatorio.TipoQuestao == TipoQuestao.InformacoesEscolares)
                        {
                            var informacoesEscolaresPersistida = ObterInformacoesEscolaresAluno(resposta);
                            if (informacoesEscolaresPersistida != null)
                                questaoRelatorio.InformacaoEscolar = informacoesEscolaresPersistida;
                            if (!questaoRelatorio.InformacaoEscolar.Any())
                                questaoRelatorio.InformacaoEscolar.Add(await mediator.Send(new ObterInformacoesEscolaresAlunoQuery(encaminhamentoAee.AlunoCodigo, encaminhamentoAee.TurmaCodigo)));
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
                    questao.Tipo == TipoQuestao.ComboMultiplaEscolha ||
                    questao.Tipo == TipoQuestao.Checkbox)
                {
                    foreach (var resposta in questao.Respostas)
                    {
                        var opcaoRespostaQuestao = questao.OpcaoResposta.FirstOrDefault(c => c.QuestaoId == questao.Id &&
                                                    c.Id == resposta.OpcaoRespostaId);

                        if (opcaoRespostaQuestao != null && opcaoRespostaQuestao.QuestoesComplementares != null && opcaoRespostaQuestao.QuestoesComplementares.Any())
                            await AdicionarQuestoesQuestionarioSecao(encaminhamentoAee, secao, 
                                                                     opcaoRespostaQuestao.QuestoesComplementares, questao.Ordem.ToString());

                    }

                }
            }
        }

        private static List<AtendimentoClinicoAlunoDto> ObterAtendimentosClinicosAluno(RespostaQuestaoDto respostaQuestao)
        {
            if (string.IsNullOrEmpty(respostaQuestao.Texto))
                return null;
            var resposta = JsonConvert.DeserializeObject<List<AtendimentoClinicoAlunoDto>>(respostaQuestao.Texto);
            return resposta;
        }

        private static List<InformacaoEscolarAlunoDto> ObterInformacoesEscolaresAluno(RespostaQuestaoDto respostaQuestao)
        {
            if (string.IsNullOrEmpty(respostaQuestao.Texto))
                return null;
            var resposta = JsonConvert.DeserializeObject<List<InformacaoEscolarAlunoDto>>(respostaQuestao.Texto);
            return resposta;
        }

    }
}
