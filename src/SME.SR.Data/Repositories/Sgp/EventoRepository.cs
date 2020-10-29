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
    public class EventoRepository : IEventoRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public EventoRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<IEnumerable<Evento>> ObterEventosPorTipoCalendarioId(long tipoCalendarioId)
        {
            var query = @"select
	                        data_inicio as DataInicio,
	                        data_fim as DataFim,
	                        letivo,
	                        e.nome,
	                        e.descricao,
                            e.ue_id as UeId,
                            e.dre_id as DreId,
                            e.tipo_evento as TipoEvento
                        from
	                        evento e
                        where
                        e.tipo_calendario_id = @tipoCalendarioId
                        and not e.excluido";
            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryAsync<Evento>(query.ToString(), new { tipoCalendarioId });
            }
        }

        public async Task<bool> EhEventoLetivoPorTipoDeCalendarioDataDreUe(long tipoCalendarioId, DateTime data, string dreId, string ueId)
        {
            string cabecalho = "select count(id) from evento e where e.excluido = false";
            string whereTipoCalendario = "and e.tipo_calendario_id = @tipoCalendarioId";

            StringBuilder query = new StringBuilder();

            ObterContadorEventosNaoLetivosSME(cabecalho, whereTipoCalendario, query, true);

            if (!string.IsNullOrEmpty(ueId))
            {
                query.AppendLine("UNION");
                ObterContadorEventosNaoLetivosUE(cabecalho, whereTipoCalendario, query, true);
            }

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                var retorno = await conexao.QueryAsync<int?>(query.ToString(), new { tipoCalendarioId, dreId, ueId, data = data.Date });
                return retorno != null && retorno.Sum() > 0;
            }

        }

        public async Task<bool> EhEventoNaoLetivoPorTipoDeCalendarioDataDreUe(long tipoCalendarioId, DateTime data, string dreId, string ueId)
        {
            string cabecalho = "select count(id) from evento e where e.excluido = false";
            string whereTipoCalendario = "and e.tipo_calendario_id = @tipoCalendarioId";

            StringBuilder query = new StringBuilder();

            ObterContadorEventosNaoLetivosSME(cabecalho, whereTipoCalendario, query, false);

            if (!string.IsNullOrEmpty(ueId))
            {
                query.AppendLine("UNION");
                ObterContadorEventosNaoLetivosUE(cabecalho, whereTipoCalendario, query, false);
            }

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                var retorno = await conexao.QueryAsync<int?>(query.ToString(), new { tipoCalendarioId, dreId, ueId, data = data.Date });
                return retorno != null && retorno.Sum() > 0;
            }
        }

        private static void ObterContadorEventosNaoLetivosSME(string cabecalho, string whereTipoCalendario, StringBuilder query, bool letivo)
        {
            var queryLetivo = letivo ? "and e.letivo in (1,3)" : "and e.letivo = 2";
            query.AppendLine(cabecalho);
            query.AppendLine(whereTipoCalendario);
            query.AppendLine("and e.dre_id is null and e.ue_id is null");
            query.AppendLine("and e.data_inicio <= @data and e.data_fim >= @data");
            query.AppendLine(queryLetivo);
        }

        private static void ObterContadorEventosNaoLetivosUE(string cabecalho, string whereTipoCalendario, StringBuilder query, bool letivo)
        {
            var queryLetivo = letivo ? "and e.letivo in (1,3)" : "and e.letivo = 2";
            query.AppendLine(cabecalho);
            query.AppendLine(whereTipoCalendario);
            query.AppendLine("and e.ue_id = @ueId");
            query.AppendLine("and e.data_inicio <= @data and e.data_fim >= @data");
            query.AppendLine(queryLetivo);
        }

        public async Task<IEnumerable<Evento>> ObterEventosPorTipoCalendarioIdEPeriodoInicioEFim(long tipoCalendarioId, DateTime periodoInicio, DateTime periodoFim)
        {
            var query = @"select
	                        data_inicio as DataInicio,
	                        data_fim as DataFim,
	                        letivo as Letivo,
                            ue_id as UeId,
                            dre_id as DreId
                        from
	                        evento
                        where
                        tipo_calendario_id = @tipoCalendarioId
                        and not excluido and data_inicio between @periodoInicio and @periodoFim;";
            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryAsync<Evento>(query.ToString(), new { tipoCalendarioId, periodoInicio, periodoFim });
            }
        }
    }
}
