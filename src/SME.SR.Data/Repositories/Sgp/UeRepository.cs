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
    public class UeRepository : IUeRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public UeRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<Ue> ObterPorCodigo(string UeCodigo)
        {
            var query = @"select Id, ue_id Codigo, Nome, tipo_escola TipoEscola from ue where ue_id = @ueCodigo";
            var parametros = new { UeCodigo };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryFirstOrDefaultAsync<Ue>(query, parametros);
            }
        }
        public async Task<IEnumerable<Ue>> ObterPorDreSemestreModadalidadeAnoId(long dreId, int? semestre, int modalidadeId, string[] anos)
        {
            var query = new StringBuilder(@"select distinct u.Id, u.ue_id Codigo, u.Nome, u.tipo_escola TipoEscola 
                            from ue u 
                            inner join turma t on t.ue_id  = u.id
                            where u.dre_id = @dreId ");

            if (semestre.HasValue)
                query.AppendLine("and t.semestre = @semestre");

            if (modalidadeId > 0)
                query.AppendLine("and t.modalidade_codigo = @modalidadeId");

            if (anos != null && anos.Length > 0)
                query.AppendLine("and t.ano = ANY(@anos)");
            
            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp);

            return await conexao.QueryAsync<Ue>(query.ToString(), new
            {
                dreId,
                semestre = semestre ?? 0,
                modalidadeId,
                anos = anos.ToList()
            });


        }
    }
}
