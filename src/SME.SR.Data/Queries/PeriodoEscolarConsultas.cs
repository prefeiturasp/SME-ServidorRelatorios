using SME.SR.Infra;
using System;
using System.Text;

namespace SME.SR.Data
{
    public class PeriodoEscolarConsultas
    {
        internal static string ObterPorTipoCalendario = @"select id, bimestre, periodo_fim PeriodoFim, 
                                                                 periodo_inicio PeriodoInicio
                    from periodo_escolar where tipo_calendario_id = @tipoCalendarioId ";

        internal static string ObterUltimoPeriodo(ModalidadeTipoCalendario modalidade, int semestre = 0) {

            var query = new StringBuilder(@"select p.bimestre, 
                        p.periodo_fim PeriodoFim, p.periodo_inicio PeriodoInicio
                            from tipo_calendario t
                         inner join periodo_escolar p on p.tipo_calendario_id = t.id
                          where t.excluido = false and t.situacao
                            and t.ano_letivo = @anoLetivo
                            and t.modalidade = @modalidade ");

            if (modalidade == ModalidadeTipoCalendario.EJA)
            {
                var periodoReferencia = semestre == 1 ? "periodo_inicio < @dataReferencia" : "periodo_fim > @dataReferencia";
                query.AppendLine($"and exists(select 0 from periodo_escolar p where tipo_calendario_id = t.id and {periodoReferencia})");
            }
            query.AppendLine("order by bimestre desc ");
            query.AppendLine("limit 1");

            return query.ToString();
        }

        internal const string ObterPorId = @"select id, bimestre, periodo_fim PeriodoFim, 
                                                    periodo_inicio PeriodoInicio
                                                from periodo_escolar where id = @idPeriodoEscolar ";
    }
}
