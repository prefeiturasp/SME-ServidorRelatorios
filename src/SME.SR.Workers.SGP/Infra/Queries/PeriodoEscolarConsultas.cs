namespace SME.SR.Workers.SGP.Infra
{
    public class PeriodoEscolarConsultas
    {
        internal static string ObterPorTipoCalendario = @"select bimestre, periodo_fim PeriodoFim, 
                                                                 periodo_inicio PeriodoInicio
                    from periodo_escolar where tipo_calendario_id = @tipoCalendarioId ";
    }
}
