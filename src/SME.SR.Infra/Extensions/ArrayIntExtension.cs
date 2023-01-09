using System.Linq;

namespace SME.SR.Infra
{
    public static class ArrayIntExtension
    {
        public static bool EstaFiltrandoTodas(this int[] array)
        {
            return (array != null && array.Any(s => s.Equals(-99))) || array == null;            
        }        
    }
}