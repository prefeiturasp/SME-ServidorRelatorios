namespace SME.SR.Data
{
    public class UeConsultas
    {
        internal static string ObterPorCodigo =
         @"select Id, ue_id Codigo, Nome, tipo_escola TipoEscola from ue where ue_id = @ueCodigo";
    }
}
