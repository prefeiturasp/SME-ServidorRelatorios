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
    public class PendenciaFechamentoRepository : IPendenciaFechamentoRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public PendenciaFechamentoRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<IEnumerable<PendenciaParaFechamentoConsolidadoDto>> ObterPendenciasParaFechamentoConsolidado(string[] turmasCodigo, int[] bimestres, long[] componentesCurricularesId)
        {
            var query = new StringBuilder(@"select p.id as PendenciaId, 
                                 p.titulo as descricao, 
                                 P.tipo as tipoPendencia  
                            from pendencia_fechamento pf
                           inner join fechamento_turma_disciplina ftd on ftd.id = pf.fechamento_turma_disciplina_id
                           inner join fechamento_turma ft on ft.id = ftd.fechamento_turma_id
                           inner join turma t on t.id = ft.turma_id
                           inner join periodo_escolar pe on pe.id = ft.periodo_escolar_id
                           inner join pendencia p on p.id = pf.pendencia_id
                           where not p.excluido
                             and P.situacao = 1
                             and t.turma_id = ANY(@turmasCodigo) ");

            if (bimestres != null && bimestres.Any())
                query.AppendLine(" and pe.bimestre = ANY(@bimestres) ");

            if (componentesCurricularesId != null && componentesCurricularesId.Any())
                query.AppendLine(" and ftd.disciplina_id = ANY(@componentesCurricularesId) ");

            query.AppendLine(" order by p.criado_em");

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp);
            return await conexao.QueryAsync<PendenciaParaFechamentoConsolidadoDto>(query.ToString(), new { turmasCodigo, bimestres, componentesCurricularesId });
        }
    }
}
