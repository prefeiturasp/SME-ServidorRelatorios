using Dapper;
using Npgsql;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class DiarioBordoRepository : IDiarioBordoRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public DiarioBordoRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<DateTime?> ObterUltimoDiarioBordoProfessor(string professorRf)
        {
            var query = @"select max(d.criado_em)
                          from aula a 
                          inner join diario_bordo d on d.aula_id = a.id
                         where not a.excluido
                           and a.professor_rf = @professorRf";

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryFirstOrDefaultAsync<DateTime?>(query, new { professorRf });
            }
        }
    }
}
