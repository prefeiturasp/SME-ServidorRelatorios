﻿using System.ComponentModel.DataAnnotations;

namespace SME.SR.Infra
{
    public enum Modalidade
    {
        [Display(Name = "Infantil", ShortName = "EI")]
        Infantil = 1,
        
        [Display(Name = "EJA", ShortName = "EJA")]
        EJA = 3,
            
        [Display(Name = "CIEJA", ShortName = "CIEJA")]
        CIEJA = 4,

        [Display(Name = "Fundamental", ShortName = "EF")]
        Fundamental = 5,

        [Display(Name = "Médio", ShortName = "EM")]
        Medio = 6,

        [Display(Name = "CMCT", ShortName = "CMCT")]
        CMCT = 7,

        [Display(Name = "MOVA", ShortName = "MOVA")]
        MOVA = 8,

        [Display(Name = "ETEC", ShortName = "ETEC")]
        ETEC = 9,

        [Display(Name = "CELP", ShortName = "CELP")]
        CELP = 10
    }

    public static class ModalidadeExtensao
    {
        public static ModalidadeTipoCalendario ObterModalidadeTipoCalendario(this Modalidade modalidade)
        {
            switch (modalidade)
            {
                case Modalidade.Infantil:
                    return ModalidadeTipoCalendario.Infantil;
                case Modalidade.EJA:
                    return ModalidadeTipoCalendario.EJA;
                case Modalidade.CELP:
                    return ModalidadeTipoCalendario.CELP;
                default:
                    return ModalidadeTipoCalendario.FundamentalMedio;
            }
        }

        public static bool EhSemestral(this Modalidade modalidade)
        {
            return modalidade == Modalidade.EJA || modalidade == Modalidade.CELP;
        }

        public static bool EhCelp(this Modalidade modalidade)
        {
            return modalidade == Modalidade.CELP;
        }
    }
}
