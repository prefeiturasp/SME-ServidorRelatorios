using Microsoft.EntityFrameworkCore.Internal;
using Npgsql;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using SME.SR.Infra.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class MapeamentoEstudanteRepository : IMapeamentoEstudanteRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;
        
        public MapeamentoEstudanteRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }       

        private string ObterCondicaoDre(FiltroRelatorioMapeamentoEstudantesDto filtro) =>
                    !filtro.DreCodigo.EstaFiltrandoTodas() ? " and d.dre_id = @dreCodigo " : string.Empty;

        private string ObterCondicaoUe(FiltroRelatorioMapeamentoEstudantesDto filtro) =>
                    !filtro.UeCodigo.EstaFiltrandoTodas() ? " and u.ue_id = @ueCodigo " : string.Empty;

        private string ObterCondicaoIds(long[] ids) =>
            ids.Any() ? " and en.id = ANY(@ids) " : string.Empty;

      
        private string ObterCondicaoPortaEntrada(FiltroRelatorioMapeamentoEstudantesDto filtro)
        {
            var condicao = string.Empty;

            //if (filtro.PortaEntradaIds != null && filtro.PortaEntradaIds.Any())
                condicao += $" and qportaentrada.PortaEntradaId = ANY(@portaEntradaIds)";

            return condicao;
        }

        private string ObterCondicaoModalidade(FiltroRelatorioMapeamentoEstudantesDto filtro) =>
            filtro.Modalidade != 0 ? " and t.modalidade_codigo = @modalidade" : string.Empty;

        private string ObterCondicao(FiltroRelatorioMapeamentoEstudantesDto filtro)
        {
            var query = new StringBuilder();
            var funcoes = new List<Func<FiltroRelatorioMapeamentoEstudantesDto, string>>
            {
                ObterCondicaoModalidade,
                ObterCondicaoDre,
                ObterCondicaoUe,
                ObterCondicaoPortaEntrada
            };

            foreach (var funcao in funcoes)
                query.Append(funcao(filtro));

            return query.ToString();
        }

        public async Task<IEnumerable<EncaminhamentoNAAPASimplesDto>> ObterResumoEncaminhamentosNAAPA(FiltroRelatorioMapeamentoEstudantesDto filtro)
        {
            var query = new StringBuilder();

            query.AppendLine($@"with vw_resposta as (select q.nome_componente,
                                                            mes.mapeamento_estudante_id, 
                                                            mer.texto resposta,	
                                                            opr.nome opcao_resposta_nome,
                                                            mer.resposta_id
                                                     from mapeamento_estudante_secao mes   
                                                     join mapeamento_estudante_questao meq on mes.id = meq.mapeamento_estudante_secao_id  
                                                     join questao q on meq.questao_id = q.id 
                                                     join mapeamento_estudante_resposta mer on mer.questao_mapeamento_estudante_id = meq.id 
                                                     join secao_mapeamento_estudante secao on secao.id = mes.secao_mapeamento_estudante_id   
                                                     join questionario q2 on q2.id = q.questionario_id 
                                                     left join opcao_resposta opr on opr.id = mer.resposta_id
                                                     where q2.tipo = 9
                                                     ),
                                vw_mapeamentos as (select distinct mapeamento.*, 
			                                              qParecer.resposta as parecerConclusivoAnoAnterior,
			                                              qTurmaAnterior.resposta as turmaAnoAnterior,
			                                              qDistorcao.opcao_resposta_nome as distorcaoIdadeAnoSerie,
			                                              qmigrante.opcao_resposta_nome as migrante,
			                                              qSRMCEFAI.opcao_resposta_nome as acompanhadoSRMCEFAI,
			                                              qplanoaee.opcao_resposta_nome as possuiPlanoAEE,
			                                              qNAAPA.opcao_resposta_nome as acompanhadoNAAPA,
			                                              qPAP.resposta as participaPAP,
			                                              qMaisEducacao.resposta as participaProjetosMaisEducacao,
			                                              qProgramaSP.opcao_resposta_nome as programaSPIntegral,
			                                              qavaliacoesexternas.resposta as avaliacoesExternasProvaSP
			                                              from (select me.id, d.dre_id dreCodigo, 
			                                                           d.abreviacao as dreAbreviacao,
			                                                           u.ue_id as ueCodigo,
			                                                           u.nome as uenome,
			                                                           u.tipo_escola as tipoEscola,
			                                                           me.aluno_codigo as alunoCodigo,
			                                                           me.aluno_nome as alunoNome,
			                                                           t.id as turmaId,
			                                                           t.turma_id as turmaCodigo,
			                                                           t.nome as turmaNome,
			                                                           t.ano_letivo as anoLetivo,
			                                                           t.semestre,
			                                                           t.modalidade_codigo as modalidade,
			                                                           row_number() over (partition by me.turma_id, me.aluno_codigo order by me.bimestre desc) sequencia
			                                                    from mapeamento_estudante me 
			                                                    inner join turma t on t.id = me.turma_id
			                                                    inner join ue u on u.id = t.ue_id 
			                                                    inner join dre d on d.id = u.dre_id
			                                                    where not me.excluido
			                                                    and t.ano_letivo = 2024
			                                                    /*and u.ue_id = '094668'
			                                                    and t.modalidade_codigo = 5
			                                                    and t.semestre = 0
			                                                    and t.turma_id in ('2670819')
			                                                    and me.aluno_codigo = '6887323'*/
			                                                    ) mapeamento 
			                                              left join vw_resposta qParecer on qParecer.mapeamento_estudante_id = mapeamento.id and qParecer.nome_componente = 'PARECER_CONCLUSIVO_ANO_ANTERIOR'
			                                              left join vw_resposta qTurmaAnterior on qTurmaAnterior.mapeamento_estudante_id = mapeamento.id and qTurmaAnterior.nome_componente = 'TURMA_ANO_ANTERIOR'
			                                              left join vw_resposta qDistorcao on qDistorcao.mapeamento_estudante_id = mapeamento.id and qDistorcao.nome_componente = 'DISTORCAO_IDADE_ANO_SERIE'
			                                              left join vw_resposta qMigrante on qMigrante.mapeamento_estudante_id = mapeamento.id and qMigrante.nome_componente = 'MIGRANTE'	
			                                              left join vw_resposta qSRMCEFAI on qSRMCEFAI.mapeamento_estudante_id = mapeamento.id and qSRMCEFAI.nome_componente = 'ACOMPANHADO_SRM_CEFAI'		
			                                              left join vw_resposta qPlanoAEE on qPlanoAEE.mapeamento_estudante_id = mapeamento.id and qPlanoAEE.nome_componente = 'POSSUI_PLANO_AEE'
			                                              left join vw_resposta qNAAPA on qNAAPA.mapeamento_estudante_id = mapeamento.id and qNAAPA.nome_componente = 'ACOMPANHADO_NAAPA'	
			                                              left join vw_resposta qPAP on qPAP.mapeamento_estudante_id = mapeamento.id and qPAP.nome_componente = 'PARTICIPA_PAP'	
			                                              left join vw_resposta qMaisEducacao on qMaisEducacao.mapeamento_estudante_id = mapeamento.id and qMaisEducacao.nome_componente = 'PARTICIPA_MAIS_EDUCACAO'		
			                                              left join vw_resposta qProgramaSP on qProgramaSP.mapeamento_estudante_id = mapeamento.id and qProgramaSP.nome_componente = 'PROGRAMA_SAO_PAULO_INTEGRAL'	
			                                              left join vw_resposta qHipoteseEscrita on qHipoteseEscrita.mapeamento_estudante_id = mapeamento.id and qHipoteseEscrita.nome_componente = 'HIPOTESE_ESCRITA'	
			                                              left join vw_resposta qAvaliacoesExternas on qAvaliacoesExternas.mapeamento_estudante_id = mapeamento.id and qAvaliacoesExternas.nome_componente = 'AVALIACOES_EXTERNAS_PROVA_SP'	
			                                              left join vw_resposta qFrequencia on qFrequencia.mapeamento_estudante_id = mapeamento.id and qFrequencia.nome_componente = 'FREQUENCIA'	
			                                              where mapeamento.sequencia = 1
			                                                    /*and qparecer.resposta like '%""3""%'
			                                                    and qdistorcao.resposta_id = 1504
			                                                    and qmigrante.resposta_id = 1507
			                                                    and qPlanoAEE.resposta_id = 1511
			                                                    and qNAAPA.resposta_id = 1513
			                                                    and qPAP.resposta like '%""Não""%'
			                                                    and qProgramaSP.resposta_id = 1514
			                                                    and qhipoteseescrita.resposta = ''
			                                                    and qavaliacoesexternas.resposta = '[]'
			                                                    and qfrequencia.resposta_id = 1516*/
			                                       )	    
			                    select vw_mapeamento.*, 
			                           me.bimestre,
			                           qAnotacoesPedagogicas.resposta as anotacoesPedagogicasBimestreAnterior_Bimestre,        
			                           qAcoesRedeApoio.resposta as acoesRedeApoio_Bimestre,
			                           qAcoesRecuperacao.resposta as acoesRecuperacaoContinua_Bimestre,
			                           qHipoteseEscrita.resposta as hipoteseEscrita_Bimestre,
			                           qObsAvaliacaoProcessual.resposta as obsAvaliacaoProcessual_Bimestre,
			                           qFrequencia.opcao_resposta_nome as Frequencia_Bimestre,
			                           qQdadeBuscasAtivas.resposta as QdadeRegistrosBuscasAtivas_Bimestre
			                    from vw_mapeamentos vw_mapeamento
			                    inner join mapeamento_estudante me on me.turma_id = vw_mapeamento.turmaId and me.aluno_codigo = vw_mapeamento.alunoCodigo 
			                    left join vw_resposta qAnotacoesPedagogicas on qAnotacoesPedagogicas.mapeamento_estudante_id = me.id and qAnotacoesPedagogicas.nome_componente = 'ANOTACOES_PEDAG_BIMESTRE_ANTERIOR'
			                    left join vw_resposta qAcoesRedeApoio on qAcoesRedeApoio.mapeamento_estudante_id = me.id and qAcoesRedeApoio.nome_componente = 'ACOES_REDE_APOIO'
			                    left join vw_resposta qAcoesRecuperacao on qAcoesRecuperacao.mapeamento_estudante_id = me.id and qAcoesRecuperacao.nome_componente = 'ACOES_RECUPERACAO_CONTINUA'
			                    left join vw_resposta qHipoteseEscrita on qHipoteseEscrita.mapeamento_estudante_id = me.id and qHipoteseEscrita.nome_componente = 'HIPOTESE_ESCRITA'
			                    left join vw_resposta qObsAvaliacaoProcessual on qObsAvaliacaoProcessual.mapeamento_estudante_id = me.id and qObsAvaliacaoProcessual.nome_componente = 'OBS_AVALIACAO_PROCESSUAL'
			                    left join vw_resposta qFrequencia on qFrequencia.mapeamento_estudante_id = me.id and qFrequencia.nome_componente = 'FREQUENCIA'
			                    left join vw_resposta qQdadeBuscasAtivas on qQdadeBuscasAtivas.mapeamento_estudante_id = me.id and qQdadeBuscasAtivas.nome_componente = 'QDADE_REGISTROS_BUSCA_ATIVA'
			                    ;");

            query.AppendLine(ObterCondicao(filtro));
            query.AppendLine(" order by d.abreviacao, u.nome, DataAberturaQueixa;");

            await using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas);
            var lookup = new Dictionary<long, EncaminhamentoNAAPASimplesDto>();

            await conexao.QueryAsync<EncaminhamentoNAAPASimplesDto, OpcaoRespostaSimplesDTO, OpcaoRespostaSimplesDTO, EncaminhamentoNAAPASimplesDto>(query.ToString(),
                (encaminhamento, opcaoRespostaPortaEntrada, opcaoRespostaFluxoAlerta) =>
                {
                    EncaminhamentoNAAPASimplesDto encaminhamentoNAAPA;
                    if (!lookup.TryGetValue(encaminhamento.Id, out encaminhamentoNAAPA))
                    {
                        encaminhamentoNAAPA = encaminhamento;
                        lookup.Add(encaminhamento.Id, encaminhamento);
                    }
                    if (opcaoRespostaPortaEntrada != null && opcaoRespostaPortaEntrada.Id != 0)
                        encaminhamentoNAAPA.PortaEntrada = opcaoRespostaPortaEntrada.Nome;
                    return encaminhamentoNAAPA;
                },
                new
                {
                    anoLetivo = filtro.AnoLetivo,
                    dreCodigo = filtro.DreCodigo,
                    ueCodigo = filtro.UeCodigo,
                    modalidade = filtro.Modalidade
                }, splitOn: "id,bimestre");
            return lookup.Values;
        }

       
    }
}
