using System.Linq;

namespace SME.SR.Infra
{
    public static class ArrayStringExtension
    {
        public static bool EstaFiltrandoTodas(this string[] array)
        {
            return array != null && array.Any(s => s.Equals("-99"));
        }
    }
}
