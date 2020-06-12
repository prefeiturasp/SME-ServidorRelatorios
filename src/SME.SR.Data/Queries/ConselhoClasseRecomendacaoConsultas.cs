namespace SME.SR.Data
{
    public class ConselhoClasseRecomendacaoConsultas
    {
        internal static string Listar = @"select recomendacao, tipo 
            from conselho_classe_recomendacao where excluido = false";
    }
}
