using Dapper;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Data.Extensions
{
    public static class StringExtension
    {
        public static int ToDbInt(this string value)
        {
            int number = 0;
            Int32.TryParse(value, out number);
            return number;
        }

        public static DbString ToDbChar(this string value, int length)
        {
            var dbChar = new DbString { Value = value, Length = length, IsFixedLength = true, IsAnsi = true };
            return dbChar;
        }

        public static DbString ToDbVarChar(this string value, int length)
        {
            var dbVarChar = new DbString { Value = value, Length = length, IsFixedLength = false, IsAnsi = true };
            return dbVarChar;
        }

        public static IEnumerable<DbString> ToDbCharList(this string[] values, int length)
        {
            foreach (var value in values)
            {
                yield return new DbString { Value = value, Length = length, IsFixedLength = true, IsAnsi = true };
            }
        }

        public static IEnumerable<DbString> ToDbVarCharList(this string[] values, int length)
        {
            foreach (var value in values)
            {
                yield return new DbString { Value = value, Length = length, IsFixedLength = false, IsAnsi = true };
            }
        }

        public static bool EhIdComponenteCurricularTerritorioSaberAgrupado(this string source)
        {
            long componenteCurricularTerritorioSaberAgrupadoId = 0;
            if (long.TryParse(source, out componenteCurricularTerritorioSaberAgrupadoId))
            {
                return componenteCurricularTerritorioSaberAgrupadoId.EhIdComponenteCurricularTerritorioSaberAgrupado();
            }
            return false;
        }

        public static bool EhIdComponenteCurricularTerritorioSaberAgrupado(this long source)
        {
            return source >= TerritorioSaberConstants.COMPONENTE_AGRUPAMENTO_TERRITORIO_SABER_ID_INICIAL;
        }
    }
}
