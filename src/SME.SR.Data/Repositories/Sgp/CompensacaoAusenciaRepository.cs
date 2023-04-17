using Dapper;
using Npgsql;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class CompensacaoAusenciaRepository : ICompensacaoAusenciaRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public CompensacaoAusenciaRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }
        public async Task<IEnumerable<RelatorioCompensacaoAusenciaRetornoConsulta>> ObterPorUeModalidadeSemestreComponenteBimestre(long UeId, int modalidadeId, int? semestre,
            string turmaCodigo, long[] componetesCurricularesIds, int bimestre, int anoLetivo)
        {

            var query = new StringBuilder(@"select ca.disciplina_id as disciplinaId, ca.bimestre, ca.nome as AtividadeNome, t.turma_id as turmaCodigo, t.nome as turmaNome,
	                       t.nome as turmaNome, t.id as turmaId, caa.qtd_faltas_compensadas as faltascompensadas, caa.codigo_aluno as alunoCodigo
                    from compensacao_ausencia ca 
                    inner join turma t on t.id  = ca.turma_id 
                    inner join ue u on t.ue_id  = u.id
                    inner join compensacao_ausencia_aluno caa on caa.compensacao_ausencia_id  = ca.id and not caa.excluido
                    where u.id = @UeId and t.modalidade_codigo = @modalidadeId and t.ano_letivo = @anoLetivo 
                    and not ca.excluido ");

            if (semestre.HasValue)
                query.AppendLine("and t.semestre = @semestre");

            if (!string.IsNullOrEmpty(turmaCodigo))
                query.AppendLine("and t.turma_id = @turmaCodigo");

            if (componetesCurricularesIds != null && componetesCurricularesIds.Length > 0)
                query.AppendLine("and ca.disciplina_id = ANY(@ComponetesCurricularesIds)");

            if (bimestre > 0)
                query.AppendLine("and ca.bimestre = @bimestre");


            var parametros = new
            {
                semestre = semestre ?? 0,
                bimestre,
                turmaCodigo,
                componetesCurricularesIds = componetesCurricularesIds.Select(a => a.ToString()).ToList(),
                modalidadeId,
                UeId,
                anoLetivo
            };

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas);
            return await conexao.QueryAsync<RelatorioCompensacaoAusenciaRetornoConsulta>(query.ToString(), parametros);
        }

    }
}
