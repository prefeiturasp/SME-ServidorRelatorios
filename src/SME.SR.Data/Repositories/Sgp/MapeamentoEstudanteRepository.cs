using Microsoft.EntityFrameworkCore.Internal;
using Npgsql;
using Org.BouncyCastle.Ocsp;
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
        public const string PARECER_CONCLUSIVO_ANO_ANTERIOR = "PARECER_CONCLUSIVO_ANO_ANTERIOR";
        public const string TURMA_ANO_ANTERIOR = "TURMA_ANO_ANTERIOR";
        public const string ANOTACOES_PEDAG_BIMESTRE_ANTERIOR = "ANOTACOES_PEDAG_BIMESTRE_ANTERIOR";
        public const string DISTORCAO_IDADE_ANO_SERIE = "DISTORCAO_IDADE_ANO_SERIE";
        public const string MIGRANTE = "MIGRANTE";
        public const string ACOMPANHADO_SRM_CEFAI = "ACOMPANHADO_SRM_CEFAI";
        public const string POSSUI_PLANO_AEE = "POSSUI_PLANO_AEE";
        public const string ACOMPANHADO_NAAPA = "ACOMPANHADO_NAAPA";
        public const string ACOES_REDE_APOIO = "ACOES_REDE_APOIO";
        public const string ACOES_RECUPERACAO_CONTINUA = "ACOES_RECUPERACAO_CONTINUA";
        public const string PARTICIPA_PAP = "PARTICIPA_PAP";
        public const string PARTICIPA_MAIS_EDUCACAO = "PARTICIPA_MAIS_EDUCACAO";
        public const string PROJETO_FORTALECIMENTO_APRENDIZAGENS = "PROJETO_FORTALECIMENTO_APRENDIZAGENS";
        public const string PROGRAMA_SAO_PAULO_INTEGRAL = "PROGRAMA_SAO_PAULO_INTEGRAL";
        public const string HIPOTESE_ESCRITA = "HIPOTESE_ESCRITA";
        public const string AVALIACOES_EXTERNAS_PROVA_SP = "AVALIACOES_EXTERNAS_PROVA_SP";
        public const string OBS_AVALIACAO_PROCESSUAL = "OBS_AVALIACAO_PROCESSUAL";
        public const string FREQUENCIA = "FREQUENCIA";
        public const string QDADE_REGISTROS_BUSCA_ATIVA = "QDADE_REGISTROS_BUSCA_ATIVA";
        
        public MapeamentoEstudanteRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }       

        private string ObterCondicaoUe(FiltroRelatorioMapeamentoEstudantesDto filtro) =>
                    !filtro.UeCodigo.EstaFiltrandoTodas() ? " and u.ue_id = @ueCodigo " : string.Empty;


        private string ObterCondicaoPareceresConclusivos(FiltroRelatorioMapeamentoEstudantesDto filtro)
        {
            var condicao = string.Empty;
            if (filtro.PareceresConclusivosIdAnoAnterior != null
                && filtro.PareceresConclusivosIdAnoAnterior.Any())
            {
                condicao += " and (";
                filtro.PareceresConclusivosIdAnoAnterior.Select((id, index) => (id, index))
                   .ToList()
                   .ForEach(tuple => {
                       condicao += $" {(tuple.index > 0 ? "or" : string.Empty)} qparecer.resposta like '%\"{tuple.id}\"%' ";
                   });                       
                condicao += " )";
            } 
                
            return condicao;
        }

        private string ObterCondicaoDistorcao(FiltroRelatorioMapeamentoEstudantesDto filtro)
        {
            var condicao = string.Empty;
            if (filtro.OpcaoRespostaIdDistorcaoIdadeAnoSerie.HasValue)
                condicao += $" and qdistorcao.resposta_id = {filtro.OpcaoRespostaIdDistorcaoIdadeAnoSerie.Value}";
            return condicao;
        }

        private string ObterCondicaoPossuiPlanoAEE(FiltroRelatorioMapeamentoEstudantesDto filtro)
        {
            var condicao = string.Empty;
            if (filtro.OpcaoRespostaIdPossuiPlanoAEE.HasValue)
                condicao += $" and qPlanoAEE.resposta_id = {filtro.OpcaoRespostaIdPossuiPlanoAEE.Value}";
            return condicao;
        }

        private string ObterCondicaoAcompanhadoNAAPA(FiltroRelatorioMapeamentoEstudantesDto filtro)
        {
            var condicao = string.Empty;
            if (filtro.OpcaoRespostaIdAcompanhadoNAAPA.HasValue)
                condicao += $" and qNAAPA.resposta_id = {filtro.OpcaoRespostaIdAcompanhadoNAAPA.Value}";
            return condicao;
        }

        private string ObterCondicaoParticipaPAP(FiltroRelatorioMapeamentoEstudantesDto filtro)
        {
            var condicao = string.Empty;
            if (filtro.ParticipaPAP.HasValue)
                if (filtro.ParticipaPAP.Value)
                  condicao += $" and qPAP.resposta !like '%\"Não\"%";
                else
                  condicao += $" and qPAP.resposta like '%\"Não\"%";
            return condicao;
        }

        private string ObterCondicaoProgramaSPIntegral(FiltroRelatorioMapeamentoEstudantesDto filtro)
        {
            var condicao = string.Empty;
            if (filtro.OpcaoRespostaIdProgramaSPIntegral.HasValue)
              condicao += $" and qProgramaSP.resposta_id = {filtro.OpcaoRespostaIdProgramaSPIntegral.Value}";
            return condicao;
        }

        private string ObterCondicaoHipoteseEscrita(FiltroRelatorioMapeamentoEstudantesDto filtro)
        {
            var condicao = string.Empty;
            if (!String.IsNullOrEmpty(filtro.OpcaoRespostaHipoteseEscrita))
                condicao += $" and qhipoteseescrita.resposta = '{filtro.OpcaoRespostaHipoteseEscrita}' ";
            return condicao;
        }

        private string ObterCondicaoAvaliacoesExternasProvaSP(FiltroRelatorioMapeamentoEstudantesDto filtro)
        {
            var condicao = string.Empty;
            if (!String.IsNullOrEmpty(filtro.OpcaoRespostaAvaliacaoExternaProvaSP))
                condicao += $" and qavaliacoesexternas.resposta like '%\"{filtro.OpcaoRespostaAvaliacaoExternaProvaSP}\"% ";
            return condicao;
        }

        private string ObterCondicaoFrequencia(FiltroRelatorioMapeamentoEstudantesDto filtro)
        {
            var condicao = string.Empty;
            if (filtro.OpcaoRespostaIdFrequencia.HasValue)
                condicao += $" and qfrequencia.resposta_id = {filtro.OpcaoRespostaIdFrequencia.Value}";
            return condicao;
        }

        private string ObterCondicoesRespostas(FiltroRelatorioMapeamentoEstudantesDto filtro)
        {
            var query = new StringBuilder();
            var funcoes = new List<Func<FiltroRelatorioMapeamentoEstudantesDto, string>>
            {
                ObterCondicaoPareceresConclusivos,
                ObterCondicaoDistorcao,
                ObterCondicaoPossuiPlanoAEE,
                ObterCondicaoAcompanhadoNAAPA,
                ObterCondicaoParticipaPAP,
                ObterCondicaoProgramaSPIntegral,
                ObterCondicaoHipoteseEscrita,
                ObterCondicaoAvaliacoesExternasProvaSP,
                ObterCondicaoFrequencia
            };

            foreach (var funcao in funcoes)
                query.Append(funcao(filtro));

            return query.ToString();
        }

        public async Task<IEnumerable<MapeamentoEstudanteUltimoBimestreDto>> ObterMapeamentosEstudantesFiltro(FiltroRelatorioMapeamentoEstudantesDto filtro)
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
                                                     where q2.tipo = {(int)TipoQuestionario.MapeamentoEstudante}
                                                     ),
                                vw_mapeamentos as (select distinct mapeamento.*, 
			                                              qParecer.resposta as parecerConclusivoAnoAnterior,
			                                              qTurmaAnterior.resposta as turmaAnoAnterior,
			                                              qDistorcao.opcao_resposta_nome as distorcaoIdadeAnoSerie,
			                                              qmigrante.resposta as Nacionalidade,
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
			                                                    and t.ano_letivo = @anoLetivo
			                                                    and u.ue_id = @ueCodigo
			                                                    and t.modalidade_codigo = @modalidade
			                                                    {(filtro.Semestre.HasValue ? " and t.semestre = @semestre " : string.Empty)}
			                                                    and t.turma_id = ANY(@turmasCodigo)
                                                                {(!string.IsNullOrEmpty(filtro.AlunoCodigo) ? " and me.aluno_codigo = @alunoCodigo " : string.Empty)}
			                                                    ) mapeamento 
			                                              left join vw_resposta qParecer on qParecer.mapeamento_estudante_id = mapeamento.id and qParecer.nome_componente = '{PARECER_CONCLUSIVO_ANO_ANTERIOR}'
			                                              left join vw_resposta qTurmaAnterior on qTurmaAnterior.mapeamento_estudante_id = mapeamento.id and qTurmaAnterior.nome_componente = '{TURMA_ANO_ANTERIOR}'
			                                              left join vw_resposta qDistorcao on qDistorcao.mapeamento_estudante_id = mapeamento.id and qDistorcao.nome_componente = '{DISTORCAO_IDADE_ANO_SERIE}'
			                                              left join vw_resposta qMigrante on qMigrante.mapeamento_estudante_id = mapeamento.id and qMigrante.nome_componente = '{MIGRANTE}'	
			                                              left join vw_resposta qSRMCEFAI on qSRMCEFAI.mapeamento_estudante_id = mapeamento.id and qSRMCEFAI.nome_componente = '{ACOMPANHADO_SRM_CEFAI}'		
			                                              left join vw_resposta qPlanoAEE on qPlanoAEE.mapeamento_estudante_id = mapeamento.id and qPlanoAEE.nome_componente = '{POSSUI_PLANO_AEE}'
			                                              left join vw_resposta qNAAPA on qNAAPA.mapeamento_estudante_id = mapeamento.id and qNAAPA.nome_componente = '{ACOMPANHADO_NAAPA}'	
			                                              left join vw_resposta qPAP on qPAP.mapeamento_estudante_id = mapeamento.id and qPAP.nome_componente = '{PARTICIPA_PAP}'	
			                                              left join vw_resposta qMaisEducacao on qMaisEducacao.mapeamento_estudante_id = mapeamento.id and qMaisEducacao.nome_componente = '{PARTICIPA_MAIS_EDUCACAO}'		
			                                              left join vw_resposta qProgramaSP on qProgramaSP.mapeamento_estudante_id = mapeamento.id and qProgramaSP.nome_componente = '{PROGRAMA_SAO_PAULO_INTEGRAL}'	
			                                              left join vw_resposta qHipoteseEscrita on qHipoteseEscrita.mapeamento_estudante_id = mapeamento.id and qHipoteseEscrita.nome_componente = '{HIPOTESE_ESCRITA}'	
			                                              left join vw_resposta qAvaliacoesExternas on qAvaliacoesExternas.mapeamento_estudante_id = mapeamento.id and qAvaliacoesExternas.nome_componente = '{AVALIACOES_EXTERNAS_PROVA_SP}'	
			                                              left join vw_resposta qFrequencia on qFrequencia.mapeamento_estudante_id = mapeamento.id and qFrequencia.nome_componente = '{FREQUENCIA}'	
			                                              where mapeamento.sequencia = 1
			                                                    {ObterCondicoesRespostas(filtro)}
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
			                    left join vw_resposta qAnotacoesPedagogicas on qAnotacoesPedagogicas.mapeamento_estudante_id = me.id and qAnotacoesPedagogicas.nome_componente = '{ANOTACOES_PEDAG_BIMESTRE_ANTERIOR}'
			                    left join vw_resposta qAcoesRedeApoio on qAcoesRedeApoio.mapeamento_estudante_id = me.id and qAcoesRedeApoio.nome_componente = '{ACOES_REDE_APOIO}'
			                    left join vw_resposta qAcoesRecuperacao on qAcoesRecuperacao.mapeamento_estudante_id = me.id and qAcoesRecuperacao.nome_componente = '{ACOES_RECUPERACAO_CONTINUA}'
			                    left join vw_resposta qHipoteseEscrita on qHipoteseEscrita.mapeamento_estudante_id = me.id and qHipoteseEscrita.nome_componente = '{HIPOTESE_ESCRITA}'
			                    left join vw_resposta qObsAvaliacaoProcessual on qObsAvaliacaoProcessual.mapeamento_estudante_id = me.id and qObsAvaliacaoProcessual.nome_componente = '{OBS_AVALIACAO_PROCESSUAL}'
			                    left join vw_resposta qFrequencia on qFrequencia.mapeamento_estudante_id = me.id and qFrequencia.nome_componente = '{FREQUENCIA}'
			                    left join vw_resposta qQdadeBuscasAtivas on qQdadeBuscasAtivas.mapeamento_estudante_id = me.id and qQdadeBuscasAtivas.nome_componente = '{QDADE_REGISTROS_BUSCA_ATIVA}'
			                    ;");

            query.AppendLine(ObterCondicoesRespostas(filtro));
            query.AppendLine(" order by vw_mapeamento.turmaNome, vw_mapeamento.alunoNome, vw_mapeamento.alunoCodigo, me.bimestre ");

            await using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas);
            var lookup = new Dictionary<long, MapeamentoEstudanteUltimoBimestreDto>();

            await conexao.QueryAsync<MapeamentoEstudanteUltimoBimestreDto, RespostaBimestralMapeamentoEstudanteDto, MapeamentoEstudanteUltimoBimestreDto>(query.ToString(),
                (mapeamento, respostaBimestral) =>
                {
                    MapeamentoEstudanteUltimoBimestreDto mapeamentoEstudante;
                    if (!lookup.TryGetValue(mapeamento.Id, out mapeamentoEstudante))
                    {
                        mapeamentoEstudante = mapeamento;
                        lookup.Add(mapeamento.Id, mapeamento);
                    }
                    mapeamentoEstudante.AdicionarRespostaBimestral(respostaBimestral);
                    return mapeamentoEstudante;
                },
                new
                {
                    anoLetivo = filtro.AnoLetivo,
                    dreCodigo = filtro.DreCodigo,
                    ueCodigo = filtro.UeCodigo,
                    modalidade = filtro.Modalidade,
                    semestre = filtro.Semestre,
                    turmasCodigo = filtro.TurmasCodigo,
                    alunoCodigo = filtro.AlunoCodigo,
                }, splitOn: "id,bimestre");
            return lookup.Values;
        }

       
    }
}
