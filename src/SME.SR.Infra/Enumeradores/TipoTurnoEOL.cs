using SME.SR.Infra.Utilitarios;
using System;
using System.ComponentModel.DataAnnotations;

namespace SME.SR.Infra
{
    public enum TipoTurnoEOL
    {
        [Display(Name = "Manhã")]
        Manha = 1,
        [Display(Name = "Intermediário")]
        Intermediario = 2,
        [Display(Name = "Tarde")]
        Tarde = 3,
        [Display(Name = "Vespertino")]
        Vespertino = 4,
        [Display(Name = "Noite")]
        Noite = 5,
        [Display(Name = "Integral")]
        Integral = 6,
    }

    public static class TipoTurnoExtension
    {
        public static string NomeTipoTurnoEol(this int TipoTurno, string prefixo)
        {
            if (Enum.IsDefined(typeof(TipoTurnoEOL), TipoTurno))
            {
                TipoTurnoEOL tipoTurno = (TipoTurnoEOL)TipoTurno;
                return $"{prefixo}{tipoTurno.GetAttribute<DisplayAttribute>()?.GetName()}";
            }
            return String.Empty;
        }
    }
}
