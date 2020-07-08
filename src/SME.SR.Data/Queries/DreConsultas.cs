namespace SME.SR.Data
{
    public class DreConsultas
    {
        internal static string ObterPorCodigo =
           @"select Id, dre_id Codigo, Abreviacao, Nome from dre where dre_id = @dreCodigo";
    }
}
