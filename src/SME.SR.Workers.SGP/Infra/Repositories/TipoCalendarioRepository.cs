using Dapper;
using Npgsql;
using SME.SR.Workers.SGP.Commons;
using SME.SR.Workers.SGP.Commons.Config;
using System;
using System.Threading.Tasks;

namespace SME.SR.Workers.SGP.Infra
{
    public class TipoCalendarioRepository : ITipoCalendarioRepository
    {
        public async Task<long> ObterPorAnoLetivoEModalidade(int anoLetivo, ModalidadeTipoCalendario modalidade, int semestre = 0)
        {
            var query = TipoCalendarioConsultas.ObterPorAnoLetivoEModalidade(modalidade, semestre);

            DateTime dataReferencia = DateTime.MinValue;
            if (modalidade == ModalidadeTipoCalendario.EJA)
            {
                dataReferencia = new DateTime(anoLetivo, semestre == 1 ? 6 : 7, 1);
            }

            var parametros = new { AnoLetivo = anoLetivo, Modalidade = (int)modalidade, DataReferencia = dataReferencia };

            using (var conexao = new NpgsqlConnection(ConnectionStrings.ConexaoSgp))
            {
                return await conexao.QuerySingleOrDefaultAsync<long>(query, parametros);
            }
        }
    }
}
