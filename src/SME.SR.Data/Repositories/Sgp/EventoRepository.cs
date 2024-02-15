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

        public async Task<IEnumerable<Evento>> ObterEventosPorTipoCalendarioIdEPeriodoInicioEFim(long tipoCalendarioId, DateTime periodoInicio, DateTime periodoFim, long? turmaId = null)
        {
            var filtroTurma = !turmaId.HasValue ? "" :
                @"inner join (
    	                select ue.ue_id, dre.dre_id from turma t
    	                inner join ue on ue.id = t.ue_id
    	                inner join dre on dre.id = ue.dre_id
    	                where t.id = @turmaId
                    ) x on e.dre_id is null 
    	                or (e.dre_id = x.dre_id and (e.ue_id is null or e.ue_id = x.ue_id))";

            var query = $@"select
                            e.id,
	                        data_inicio as DataInicio,
	                        data_fim as DataFim,
	                        e.letivo,
	                        e.nome,
	                        e.descricao,
                            e.ue_id as UeId,
                            e.dre_id as DreId,
                            e.tipo_evento_id as TipoEvento,
                            et.id,
                            et.descricao
                        from
	                        evento e
                        inner join evento_tipo et on et.id = e.tipo_evento_id
                        {filtroTurma}
                        where
                        e.tipo_calendario_id = @tipoCalendarioId
                        and not e.excluido and data_inicio between @periodoInicio and @periodoFim;";

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryAsync<Evento, EventoTipo, Evento>(query.ToString(), 
                    (evento, eventoTipo) =>
                    {
                        evento.EventoTipo = eventoTipo;
                        return evento;
                    },
                    new { tipoCalendarioId, periodoInicio, periodoFim, turmaId });
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

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
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

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
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

        public async Task<IEnumerable<Evento>> ObterEventosPorTipoDeCalendarioAsync(long tipoCalendarioId, string ueCodigo = "", params EventoLetivo[] tiposLetivosConsiderados)
        {
            bool possuiUeCodigo = !string.IsNullOrEmpty(ueCodigo);

            var query = new StringBuilder(@"select
                                                data_inicio,
                                                data_fim,
                                                letivo,
                                                e.ue_id,
                                                e.dre_id,
                                                e.nome,
                                                e.feriado_id,
                                                e.tipo_evento_id TipoEventoId
                                            from
                                                evento e
                                                    inner join tipo_calendario tc
                                                        on e.tipo_calendario_id = tc.id");
            if (possuiUeCodigo)
                query.AppendLine(@" left join ue u 
                                        on u.ue_id = e.ue_id
                                    left join dre d 
                                        on d.id = u.dre_id");

            query.AppendLine(@" where
                                    e.tipo_calendario_id = @tipoCalendarioId
                                and extract(year from e.data_inicio) = tc.ano_letivo     
                                and e.letivo = any(@tiposLetivos)                         
                                and not e.excluido");

            if (possuiUeCodigo)
                query.AppendLine($@" and (e.ue_id = @ueCodigo 
                                            or (e.ue_id is null and e.dre_id is null)
                                            or (e.ue_id is null and e.dre_id = d.dre_id))");

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryAsync<Evento>(query.ToString(), new
                {
                    tipoCalendarioId,
                    ueCodigo,
                    tiposLetivos = tiposLetivosConsiderados.Select(tlc => (int)tlc).ToArray()
                });
            }
        }
    }
}
