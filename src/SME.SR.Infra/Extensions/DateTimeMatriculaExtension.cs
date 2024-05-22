using System;

namespace SME.SR.Infra.Extensions
{
    public static class DateTimeMatriculaExtension
    {
        public static bool AntecedeMesAno(this DateTime data, int mesReferencia, int anoReferencia)
        {
            if (data.Year < anoReferencia)
                return true;

            if (data.Year == anoReferencia && data.Month < mesReferencia)
                return true;

            if (data.Year == anoReferencia && data.Month == mesReferencia && data.Day < DateTime.DaysInMonth(anoReferencia, mesReferencia))
                return true;

            return false;
        }

        public static bool PosteriorOuEquivalenteMesAno(this DateTime data, int mesReferencia, int anoReferencia)
        {
            if (data.Year > anoReferencia)
                return true;

            if (data.Year == anoReferencia && data.Month >= mesReferencia)
                return true;

            mesReferencia = mesReferencia == 1 ? 12 : mesReferencia - 1;
            if (mesReferencia == 12)
                anoReferencia = -1;

            if (data.Year == anoReferencia && data.Month == mesReferencia && data.Day == DateTime.DaysInMonth(anoReferencia, mesReferencia))
                return true;

            return false;
        }
    }
}
