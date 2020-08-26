using System.ComponentModel.DataAnnotations;

namespace SME.SR.Infra
{
    public enum TipoNota
    {
        [Display(Name = "Nota")]
        Nota = 1,
        [Display(Name = "Conceito")]
        Conceito = 2,
        [Display(Name = "Sintese")]
        Sintese = 3
    }
}
