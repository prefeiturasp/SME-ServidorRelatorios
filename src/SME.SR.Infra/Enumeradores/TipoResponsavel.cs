using System.ComponentModel.DataAnnotations;

namespace SME.SR.Infra
{
    public enum TipoResponsavel
    {
        [Display(Name = "Filiação 1")]
        Filiacao1 = 1,

        [Display(Name = "Filiação 2")]
        Filiacao2 = 2,

        [Display(Name = "Responsável Legal")]
        RL = 3,

        [Display(Name = "O próprio estudante")]
        Estudante = 4,

    }
}
