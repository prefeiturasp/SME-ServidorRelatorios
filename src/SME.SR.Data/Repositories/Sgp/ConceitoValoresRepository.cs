using Dapper;
using Npgsql;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SME.SR.Data.Interfaces;

namespace SME.SR.Data
{
    public class ConceitoValoresRepository : IConceitoValoresRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;
        
        public ConceitoValoresRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<IEnumerable<ConceitoDto>> ObterDadosLegendaHistoricoEscolar(TipoLegenda tipoLegenda)
        {

            string query;
            if (tipoLegenda == TipoLegenda.Conceito)
            {
                query = @"select RTRIM(LTRIM(cs.valor)) as valor, 
                        RTRIM(LTRIM(cs.descricao)) as descricao 
                        from conceito_valores cs
                        where ativo = true;";
            } else
            {
                query = @"select RTRIM(LTRIM(sv.valor)) as valor, 
                        RTRIM(LTRIM(sv.descricao)) as descricao 
                        from sintese_valores sv
                        where ativo = true;";
            }

            var parametros = new { };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryAsync<ConceitoDto>(query, parametros);
            }
        }
    }
}
