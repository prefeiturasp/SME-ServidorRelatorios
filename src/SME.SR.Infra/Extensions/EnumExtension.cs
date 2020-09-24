using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SME.SR.Infra.Extensions
{
    public static class EnumExtension
    {
        public static bool EhUmDosValores(this Enum valorEnum, params Enum[] valores)
        {
            return valores.Contains(valorEnum);
        }

        public static string ShortName(this Enum enumValue)
            => enumValue.GetAttribute<DisplayAttribute>().ShortName;

        public static string Description(this Enum enumValue)
           => enumValue.GetAttribute<DisplayAttribute>().Description;

        public static Dictionary<Enum, string> ToDictionary<TEnum>()
            where TEnum : struct
        {
            if (!typeof(TEnum).IsEnum) throw new InvalidOperationException();

            return ((TEnum[])Enum.GetValues(typeof(TEnum))).Cast<Enum>().ToDictionary(key => key, value => value.Name());
        }
    }
}
