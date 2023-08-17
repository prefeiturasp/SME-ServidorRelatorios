using System.ComponentModel.DataAnnotations;

namespace SME.SR.Infra
{
    public enum TipoConceito
    {
        [Display(Name = "P")]
        PlenamenteSatisfatorio = 1,
        [Display(Name = "S")]
        Satisfatorio = 2,
        [Display(Name = "NS")]
        NaoSatisfatorio = 3
    }
}
