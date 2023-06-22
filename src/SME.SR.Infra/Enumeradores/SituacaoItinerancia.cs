using System.ComponentModel.DataAnnotations;

namespace SME.SR.Infra
{
    public enum SituacaoItinerancia
    {
        [Display(Name = "Digitado")]
        Digitado = 1,
        [Display(Name = "Aceito")]
        Aceito = 2,
        [Display(Name = "Reprovado")]
        Reprovado = 3,
        [Display(Name = "Enviado")]
        Enviado = 4,
        [Display(Name = "Aguardando aprovação")]
        AguardandoAprovacao = 5
    }
}
