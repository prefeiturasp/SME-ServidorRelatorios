namespace SME.SR.Infra.Extensions
{
    public static class IntExtension
    {
        public static int ArredondaParaProximaDezena(this int numero)
        {
            if (numero % 10 == 0)
                return numero;

            while (numero % 10 != 0)
            {
                numero += 1;
            }
            return numero;
        }
        
        public static bool EstaFiltrandoTodas(this int filtro)
        {
            return filtro == -99;
        }
    }
}
