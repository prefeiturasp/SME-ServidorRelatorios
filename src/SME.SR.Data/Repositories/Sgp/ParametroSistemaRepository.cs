using Dapper;
using Npgsql;
using SME.SR.Data.Interfaces;
using SME.SR.Data.Models;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class ParametroSistemaRepository : IParametroSistemaRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public ParametroSistemaRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<IEnumerable<MediaFrequencia>> ObterMediasFrequencia()
        {
            var query = @"select 
                         valor Media, tipo, Ano from parametros_sistema
                         where ativo and tipo = ANY(@tipos) ";
            var parametros = new { Tipos = new int[] { (int)TipoParametroSistema.CompensacaoAusenciaPercentualFund2, (int)TipoParametroSistema.CompensacaoAusenciaPercentualRegenciaClasse } };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryAsync<MediaFrequencia>(query, parametros);
            }
        }

        public async Task<string> ObterValorPorTipo(TipoParametroSistema tipo)
        {
            var query = ParametroSistemaConsultas.ObterValor;
            var parametros = new { Tipo = tipo };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryFirstOrDefaultAsync<string>(query, parametros);
            }
        }

        public async Task<string> ObterValorPorAnoTipo(int ano, TipoParametroSistema tipo)
        {
            var query = @"select valor from parametros_sistema
                         where ativo 
                           and tipo = @tipo
                           and ano = @ano";

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryFirstOrDefaultAsync<string>(query, new { ano, tipo });
            }
        }

        public async Task<ParametroSistemaAnoSituacaoDto> VerificarSeParametroEstaAtivo(TipoParametroSistema tipo)
        {
            var query = @"select ativo, ano from parametros_sistema
                         where ativo 
                           and tipo = @tipo";

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryFirstOrDefaultAsync<ParametroSistemaAnoSituacaoDto>(query, new { tipo });
            }
        }
    }
}
