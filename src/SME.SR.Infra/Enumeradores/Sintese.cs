using System.ComponentModel.DataAnnotations;

namespace SME.SR.Infra
{
    public enum Sintese
    {
        [Display(Name = "Frequente")]
        Frequente = 1,

        [Display(Name = "Não frequente")]
        NaoFrequente = 2
    }
}
