using System.ComponentModel.DataAnnotations;

namespace SME.SR.Infra
{
    public enum ProficienciaSondagemEnum
    {
        [Display(Name = "Autoral")]
        Autoral = 0,
        [Display(Name = "Aditivo")]
        CampoAditivo = 1,
        [Display(Name = "Multiplicativo")]
        CampoMultiplicativo = 2,
        [Display(Name = "Números")]
        Numeros = 3,
        [Display(Name = "Leitura")]
        Leitura = 4,
        [Display(Name = "Escrita")]
        Escrita = 5,
        [Display(Name = "Leitura em voz alta")]
        LeituraVozAlta = 6 ,       
        [Display(Name = "IAD")]
        IAD = 99
    }
}
