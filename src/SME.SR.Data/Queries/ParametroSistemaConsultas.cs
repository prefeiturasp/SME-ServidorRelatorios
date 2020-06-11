namespace SME.SR.Data
{
    public class ParametroSistemaConsultas
    {
        internal static string ObterValor = @"select 
                         valor from parametros_sistema
                         where ativo and tipo = @tipo ";
    }
}
