using System.ComponentModel.DataAnnotations;

namespace SME.SR.Infra
{
    public enum TipoConceito
    {
        [Display(Name = "P")]
        Nota = 1,
        [Display(Name = "S")]
        Conceito = 2,
        [Display(Name = "NS")]
        Sintese = 3
    }
}
