using System.ComponentModel.DataAnnotations;

namespace SME.SR.Infra
{
    public enum EtapaEnsino
    {
        [Display(Name = "EDUCACAO INFANTIL", ShortName = "ED INF")]
        EducacaoInfantil = 1,
        [Display(Name = "EJA CIEJA", ShortName = "EJA CIEJA")]
        EjaCieja = 2,
        [Display(Name = "EJA ESCOLAS ENSINO FUNDAMENTAL", ShortName = "EJA")]
        EjaEnsinoFundamental = 3,
        [Display(Name = "ENSINO FUNDAMENTAL DE 8 ANOS", ShortName = "ENS FUND8A")]
        Fundamental8Anos = 4,
        [Display(Name = "ENSINO FUNDAMENTAL DE 9 ANOS", ShortName = "ENS FUND9A")]
        Fundamental9Anos = 5,
        [Display(Name = "ENSINO MEDIO", ShortName = "ENS MEDIO")]
        EnsinoMedio = 6,
        [Display(Name = "ENSINO MEDIO EJA", ShortName = "EJA MEDIO")]
        EnsinoMedioEja = 7,
        [Display(Name = "ENSINO MEDIO INTEGRADO", ShortName = "MEDIO INT")]
        EnsinoMedioIntegrado = 8,
        [Display(Name = "ENSINO MEDIO NORMAL/MAGISTERIO", ShortName = "NORMAL")]
        Magisterio = 9,
        [Display(Name = "EDUCACAO INFANTIL", ShortName = "ED INF ESP")]
        EducacaoInfantilEspecial = 10,
        [Display(Name = "EJA ESCOLAS EDUCACAO ESPECIAL", ShortName = "EJA ESP")]
        EjaEducacaoInfantilEspecial = 11,
        [Display(Name = "ENSINO FUNDAMENTAL 8 ANOS", ShortName = "FUND ESP 8")]
        Fundamental8AnosEspecial = 12,
        [Display(Name = "ENSINO FUNDAMENTAL 9 ANOS", ShortName = "FUND ESP 9")]
        Fundamental9AnosEspecial = 13,
        [Display(Name = "TECNICO MEDIO", ShortName = "TEC MEDIO")]
        TecnicoMedio = 14,
        [Display(Name = "PROJOVEM", ShortName = "PROJOVEM")]
        Projovem = 16,
        [Display(Name = "ESPEC ENS MEDIO", ShortName = "ESP MEDIO")]
        EnsinoMedioEspecial = 17,
    }
}
