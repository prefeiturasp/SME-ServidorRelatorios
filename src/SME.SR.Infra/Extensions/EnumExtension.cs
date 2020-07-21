using System;
using System.Linq;

namespace SME.SR.Infra.Extensions
{
    public static class EnumExtension
    {
        public static bool EhUmDosValores(this Enum valorEnum, params Enum[] valores)
        {
            return valores.Contains(valorEnum);
        }
    }
}
