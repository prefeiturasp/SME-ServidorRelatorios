using Dapper;
using Npgsql;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class ConselhoClasseRepository : IConselhoClasseRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public ConselhoClasseRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<long> ObterConselhoPorFechamentoTurmaId(long fechamentoTurmaId)
        {
            var query = ConselhoClasseConsultas.ConselhoPorFechamentoId;
            var parametros = new { FechamentoTurmaId = fechamentoTurmaId };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryFirstOrDefaultAsync<long>(query, parametros);
            }
        }
        public async Task<IEnumerable<long>> ObterPareceresConclusivosPorTipoAprovacao(bool aprovado)
        {
            var query = @"select id from conselho_classe_parecer ccp 
                            where 
                            ccp.aprovado  = @aprovado";

            var parametros = new { aprovado };

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp);

            return await conexao.QueryAsync<long>(query, parametros);
        }
    }
}
