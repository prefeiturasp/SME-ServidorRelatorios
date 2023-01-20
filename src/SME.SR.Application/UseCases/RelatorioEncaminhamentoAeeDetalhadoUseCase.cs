using MediatR;
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

           /*

             await mediator.Send(new GerarRelatorioHtmlPDFEncaminhamentoAeeCommand(cabecalho, encaminhamentosAgrupados, request.CodigoCorrelacao));*/
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
            var questoesRelatorio = await ObterQuestoesQuestionario(questoes.ToList());
            relatorioEncaminhamentoAeeDetalhado.Detalhes.InformacoesEscolares = new SecaoQuestoesEncaminhamentoAeeDto() { NomeComponenteSecao = SECAO_INFORMACOES_ESCOLARES, Questoes = questoesRelatorio } ;

            questoes = await mediator.Send(new ObterQuestoesEncaminhamentoAEEPorIdENomeComponenteSecaoQuery(encaminhamentoAee.Id, SECAO_DESCRICAO_ENCAMINHAMENTO));
            questoesRelatorio = await ObterQuestoesQuestionario(questoes.ToList());
            relatorioEncaminhamentoAeeDetalhado.Detalhes.DescricaoEncaminhamento = new SecaoQuestoesEncaminhamentoAeeDto() { NomeComponenteSecao = SECAO_DESCRICAO_ENCAMINHAMENTO, Questoes = questoesRelatorio };

            questoes = await mediator.Send(new ObterQuestoesEncaminhamentoAEEPorIdENomeComponenteSecaoQuery(encaminhamentoAee.Id, SECAO_PARECER_COORDENACAO));
            questoesRelatorio = await ObterQuestoesQuestionario(questoes.ToList());
            relatorioEncaminhamentoAeeDetalhado.Detalhes.ParecerCoordenacao = new SecaoQuestoesEncaminhamentoAeeDto() { NomeComponenteSecao = SECAO_PARECER_COORDENACAO, Questoes = questoesRelatorio };

            questoes = await mediator.Send(new ObterQuestoesEncaminhamentoAEEPorIdENomeComponenteSecaoQuery(encaminhamentoAee.Id, SECAO_PARECER_AEE));
            questoesRelatorio = await ObterQuestoesQuestionario(questoes.ToList());
            relatorioEncaminhamentoAeeDetalhado.Detalhes.ParecerAee = new SecaoQuestoesEncaminhamentoAeeDto() { NomeComponenteSecao = SECAO_PARECER_AEE, Questoes = questoesRelatorio };


        }

        private async Task<List<QuestaoEncaminhamentoAeeDto>> ObterQuestoesQuestionario(List<QuestaoDto> questoes)
        {
            var questoesRelatorio = new List<QuestaoEncaminhamentoAeeDto>();

            foreach (var questao in questoes)
            {
                var questaoRelatorio = new QuestaoEncaminhamentoAeeDto
                {
                    Questao = questao.Nome,
                    Ordem = questao.Ordem,
                    QuestaoId = questao.Id,
                    TipoQuestao = questao.Tipo
                };

                var respostaQuestao = questao.Respostas.FirstOrDefault(c => c.QuestaoId == questao.Id);

                if (respostaQuestao == null)
                    continue;

                questaoRelatorio.RespostaId = respostaQuestao.OpcaoRespostaId;
                questaoRelatorio.Resposta = respostaQuestao.Texto;

                var opcaoRespostaQuestao = questao.OpcaoResposta.FirstOrDefault(c => c.QuestaoId == questao.Id &&
                    c.Id == respostaQuestao.OpcaoRespostaId);

                questaoRelatorio.Resposta = questao.Tipo switch
                {
                    TipoQuestao.Radio => opcaoRespostaQuestao?.Nome,
                };

                /*
                 
                   questaoRelatorio.Justificativa = ObterJustificativaQuestao(opcaoRespostaQuestao);
                
                var respostaFrequenciaAluno = ObterRespostaFrequenciaAluno(questao.Tipo, respostaQuestao);
                
                if (respostaFrequenciaAluno != null)
                    questaoRelatorio.FrequenciaAluno = ObterRespostaFrequenciaAluno(questao.Tipo, respostaQuestao);

                
                var respostaSrm =  ObterRespostaInformacoesSrm(questao.Tipo,respostaQuestao);

                if (respostaSrm != null)
                    questaoRelatorio.InformacoesSrm = respostaSrm;

                 */
                questoesRelatorio.Add(questaoRelatorio);
            }

            return questoesRelatorio;
        }
    }
}
