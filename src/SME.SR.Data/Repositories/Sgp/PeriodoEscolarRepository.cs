using Dapper;
using Npgsql;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class PeriodoEscolarRepository : IPeriodoEscolarRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public PeriodoEscolarRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<IEnumerable<PeriodoEscolar>> ObterPeriodosEscolaresPorTipoCalendario(long tipoCalendarioId)
        {
            var query = PeriodoEscolarConsultas.ObterPorTipoCalendario;
            var parametros = new { TipoCalendarioId = tipoCalendarioId };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryAsync<PeriodoEscolar>(query, parametros);
            }
        }

        public async Task<PeriodoEscolar> ObterUltimoPeriodoAsync(int anoLetivo, ModalidadeTipoCalendario modalidadeTipoCalendario, int semestre)
        {
            var query = PeriodoEscolarConsultas.ObterUltimoPeriodo(modalidadeTipoCalendario, semestre);

            DateTime dataReferencia = DateTime.MinValue;
            if (modalidadeTipoCalendario == ModalidadeTipoCalendario.EJA)
                dataReferencia = new DateTime(anoLetivo, semestre == 1 ? 6 : 7, 1);

            var parametros = new { AnoLetivo = anoLetivo, Modalidade = (int)modalidadeTipoCalendario, DataReferencia = dataReferencia };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryFirstOrDefaultAsync<PeriodoEscolar>(query, parametros);
            }
        }

        public async Task<PeriodoEscolar> ObterPeriodoEscolarPorId(long idPeriodoEscolar)
        {
            const string query = PeriodoEscolarConsultas.ObterPorId;
            var parametros = new { idPeriodoEscolar };

            await using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas);
            return await conexao.QueryFirstOrDefaultAsync<PeriodoEscolar>(query, parametros);
        }
    }
}
