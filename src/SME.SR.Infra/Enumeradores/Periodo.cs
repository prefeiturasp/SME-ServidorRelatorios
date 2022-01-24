using System.ComponentModel.DataAnnotations;

namespace SME.SR.Infra
{
    public enum Periodo
    {
        [Display(Name = "Anual")]
        Anual = 1,

        [Display(Name = "Semestral")]
        Semestral = 2
    }
}
