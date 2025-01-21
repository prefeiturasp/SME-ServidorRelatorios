using Microsoft.EntityFrameworkCore.Internal;
using Npgsql;
using SME.SR.Data.Extensions;
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
    public class BuscaAtivaRepository : IBuscaAtivaRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public BuscaAtivaRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        private string ObterCondicaoTurmas(FiltroRelatorioBuscasAtivasDto filtro) =>
                    filtro.TurmasCodigo.Any() ? " and t.turma_id = ANY(@turmasCodigo) " : string.Empty;

        private string ObterCondicaoUes(FiltroRelatorioBuscasAtivasDto filtro) =>
                     string.IsNullOrEmpty(filtro.UeCodigo) ? string.Empty : " and u.ue_id = @ueCodigo ";

        private string ObterCondicaoSemestre(FiltroRelatorioBuscasAtivasDto filtro) =>
                    filtro.Semestre.HasValue ? " and t.semestre = @semestre " : string.Empty;

        private string ObterCondicaoAluno(FiltroRelatorioBuscasAtivasDto filtro) =>
                    !string.IsNullOrEmpty(filtro.AlunoCodigo) ? " and raba.aluno_codigo = @alunoCodigo " : string.Empty;

        private string ObterCondicaoABAE(FiltroRelatorioBuscasAtivasDto filtro) =>
                    !string.IsNullOrEmpty(filtro.CpfABAE) ? " and raba.criado_rf = @cpfABAE " : string.Empty;

        private string ObterCondicaoPeriodo(FiltroRelatorioBuscasAtivasDto filtro) =>
                    filtro.DataInicioRegistroAcao.HasValue 
                    && filtro.DataFimRegistroAcao.HasValue 
                       ? @" and CASE WHEN qDataRegistro.resposta ~'^[0-9]{4}-[0-9]{2}-[0-9]*'
                                        THEN to_date(qDataRegistro.resposta,'yyyy-mm-dd') between @dataInicioRegistroAcao and @dataFimRegistroAcao
                                      ELSE FALSE END " : string.Empty;

        private string ObterCondicaoMotivosAusencia(FiltroRelatorioBuscasAtivasDto filtro) =>
                    filtro.OpcoesRespostaIdMotivoAusencia.Any()
                       ? @" and exists (select 1 from vw_resposta qJustificativaMotivoFalta_flt 
                                          where qJustificativaMotivoFalta_flt.registro_acao_busca_ativa_id = raba.id 
                                          and qJustificativaMotivoFalta_flt.nome_componente = 'JUSTIFICATIVA_MOTIVO_FALTA'
                                          and qJustificativaMotivoFalta_flt.resposta_id = any(@opcoesRespostaIdMotivoAusencia)) " : string.Empty;

        private string ObterCondicao(FiltroRelatorioBuscasAtivasDto filtro)
        {
            var query = new StringBuilder();
            var funcoes = new List<Func<FiltroRelatorioBuscasAtivasDto, string>>
            {
                ObterCondicaoTurmas,
                ObterCondicaoUes,
                ObterCondicaoSemestre,
                ObterCondicaoAluno,
                ObterCondicaoABAE,
                ObterCondicaoPeriodo,
                ObterCondicaoMotivosAusencia
            };

            foreach (var funcao in funcoes)
                query.Append(funcao(filtro));

            return query.ToString();
        }

        public async Task<IEnumerable<BuscaAtivaSimplesDto>> ObterResumoBuscasAtivas(FiltroRelatorioBuscasAtivasDto filtro)
        {
            var query = new StringBuilder();

            query.AppendLine($@"with vw_resposta as (select q.nome_componente,
                                                     rabas.registro_acao_busca_ativa_id, 
                                                     rabar.texto resposta,	
                                                     opr.nome opcao_resposta_nome,
                                                     rabar.resposta_id
                                              from registro_acao_busca_ativa_secao rabas  
                                              join registro_acao_busca_ativa_questao rabaq  on rabas.id = rabaq.registro_acao_busca_ativa_secao_id  
                                              join questao q on rabaq.questao_id = q.id 
                                              join registro_acao_busca_ativa_resposta rabar on rabar.questao_registro_acao_id = rabaq.id 
                                              join secao_registro_acao_busca_ativa secao on secao.id = rabas.secao_registro_acao_id 
                                              join questionario q2 on q2.id = q.questionario_id 
                                              left join opcao_resposta opr on opr.id = rabar.resposta_id
                                              where q2.tipo = {(int)TipoQuestionario.RegistroAcaoBuscaAtiva}
                                                    and not rabas.excluido 
                                                    and not rabaq.excluido 
                                                    and not rabar.excluido
                                              ) 
                             SELECT raba.id, 
                                    d.dre_id DreCodigo,
                                    d.abreviacao DreAbreviacao,
                                    u.ue_id UeCodigo,
                                    u.nome as UeNome,
                                    u.tipo_escola TipoEscola,
                                    t.turma_id TurmaCodigo,
                                    t.nome as TurmaNome,
                                    t.ano AnoTurma,
                                    t.tipo_turno as TurmaTipoTurno,
                                    t.ano_letivo AnoLetivo,  
                                    t.modalidade_codigo Modalidade,
                                    raba.aluno_codigo AlunoCodigo,
                                    raba.aluno_nome AlunoNome,
                                    to_date(qDataRegistro.resposta,'yyyy-mm-dd') as DataRegistroAcao,
                                    qProcedimentoRealizado.opcao_resposta_nome as ProcedimentoRealizado,
                                    qConseguiuContatoResp.opcao_resposta_nome as ConseguiuContatoResponsavel,
                                    case when coalesce(qObsGeralAoContatarResponsavel.resposta,'') <> '' then qObsGeralAoContatarResponsavel.resposta 
                                    else  qObsGeralAoNaoContatarResponsavel.resposta end as ObsGeralAoContatarOuNaoResponsavel,
                                    qJustificativaMotivoFalta_OpcaoOutros.resposta as JustificativaMotivoFalta_OpcaoOutros,
                                    (SELECT string_agg(opcao_resposta_nome, ' | ') 
                                     FROM vw_resposta 
                                     WHERE registro_acao_busca_ativa_id = raba.id 
                                           AND nome_componente = 'QUESTOES_OBS_DURANTE_VISITA') as QuestoesObsDuranteVisita,
                                    (SELECT string_agg(opcao_resposta_nome, ' | ') 
                                     FROM vw_resposta 
                                     WHERE registro_acao_busca_ativa_id = raba.id 
                                           AND nome_componente = 'JUSTIFICATIVA_MOTIVO_FALTA') as JustificativaMotivoFalta
                              FROM registro_acao_busca_ativa raba
                              INNER JOIN turma t ON t.id = raba.turma_id
                              INNER JOIN ue u ON u.id = t.ue_id
                              INNER JOIN dre d ON d.id = u.dre_id
                              left join vw_resposta qDataRegistro on qDataRegistro.registro_acao_busca_ativa_id = raba.id and qDataRegistro.nome_componente = 'DATA_REGISTRO_ACAO'
                              left join vw_resposta qProcedimentoRealizado on qProcedimentoRealizado.registro_acao_busca_ativa_id = raba.id and qProcedimentoRealizado.nome_componente = 'PROCEDIMENTO_REALIZADO'
                              left join vw_resposta qConseguiuContatoResp on qConseguiuContatoResp.registro_acao_busca_ativa_id = raba.id and qConseguiuContatoResp.nome_componente = 'CONSEGUIU_CONTATO_RESP'
                              left join vw_resposta qJustificativaMotivoFalta_OpcaoOutros on qJustificativaMotivoFalta_OpcaoOutros.registro_acao_busca_ativa_id = raba.id and qJustificativaMotivoFalta_OpcaoOutros.nome_componente = 'JUSTIFICATIVA_MOTIVO_FALTA_OUTROS' 
                              left join vw_resposta qObsGeralAoContatarResponsavel on qObsGeralAoContatarResponsavel.registro_acao_busca_ativa_id = raba.id and qObsGeralAoContatarResponsavel.nome_componente = 'OBS_GERAL'
                              left join vw_resposta qObsGeralAoNaoContatarResponsavel on qObsGeralAoNaoContatarResponsavel.registro_acao_busca_ativa_id = raba.id and qObsGeralAoNaoContatarResponsavel.nome_componente = 'OBS_GERAL_NAO_CONTATOU_RESP'
                              where not raba.excluido 
                                    and t.modalidade_codigo = @modalidade
                                    and t.ano_letivo = @anoLetivo ");

            query.AppendLine(ObterCondicao(filtro));
            query.AppendLine(@" group by raba.id, 
                                    d.dre_id,
                                    d.abreviacao,
                                    u.ue_id,
                                    u.nome,
                                    u.tipo_escola,
                                    t.turma_id,
                                    t.nome,
                                    t.ano,
                                    t.tipo_turno,
                                    t.ano_letivo,  
                                    t.modalidade_codigo,
                                    raba.aluno_codigo,
                                    raba.aluno_nome,
	                                qDataRegistro.resposta,
                                    qProcedimentoRealizado.opcao_resposta_nome,
                                    qConseguiuContatoResp.opcao_resposta_nome,
                                    qObsGeralAoContatarResponsavel.resposta, 
                                    qObsGeralAoNaoContatarResponsavel.resposta,
                                    qJustificativaMotivoFalta_OpcaoOutros.resposta
                                order by d.abreviacao, u.nome, t.nome, raba.aluno_nome, to_date(qDataRegistro.resposta,'yyyy-mm-dd') desc;");

            await using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas);
            
            return await conexao.QueryAsync<BuscaAtivaSimplesDto>(query.ToString(),
                new
                {
                    anoLetivo = filtro.AnoLetivo,
                    dreCodigo = filtro.DreCodigo,
                    ueCodigo = filtro.UeCodigo,
                    modalidade = (int) filtro.Modalidade,
                    semestre = filtro.Semestre,
                    turmasCodigo = filtro.TurmasCodigo,
                    alunoCodigo = filtro.AlunoCodigo,
                    cpfABAE = filtro.CpfABAE.SomenteNumeros(),
                    dataInicioRegistroAcao = filtro.DataInicioRegistroAcao,
                    dataFimRegistroAcao = filtro.DataFimRegistroAcao,
                    OpcoesRespostaIdMotivoAusencia = filtro.OpcoesRespostaIdMotivoAusencia,
                });
        }
    }
}
