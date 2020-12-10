using Dapper;
using Npgsql;
using SME.SR.Infra;
using SME.SR.Infra.Dtos.AE.Adesao;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class DashboardAdesaoRepository : IDashboardAdesaoRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public DashboardAdesaoRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }
        public async Task<IEnumerable<AdesaoAEQueryConsolidadoRetornoDto>> ObterAdesaoDashboardPorFiltros(string dreCodigo, string ueCodigo)
        {
            var query = new StringBuilder(@"select 
                            da.dre_codigo as DreCodigo,
                            da.ue_codigo as UeCodigo,
                            da.codigo_turma as TurmaCodigo,
                            da.usuarios_primeiro_acesso_incompleto as PrimeiroAcessoIncompleto,
                            da.usuarios_validos as Validos,
                            da.usuarios_cpf_invalidos as CpfsInvalidos,
                            da.usuarios_sem_app_instalado  as SemAppInstalado,
                            da.dre_nome  as DreNome
                            from dashboard_adesao da where 1=1 ");

            if (!string.IsNullOrEmpty(dreCodigo))
                query.AppendLine("and da.dre_codigo = @dreCodigo");

            if (!string.IsNullOrEmpty(ueCodigo))
                query.AppendLine("and da.ue_codigo = @ueCodigo");

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringAE))
            {
                return await conexao.QueryAsync<AdesaoAEQueryConsolidadoRetornoDto>(query.ToString(), new { dreCodigo, ueCodigo });
            }
        }
    }
}
