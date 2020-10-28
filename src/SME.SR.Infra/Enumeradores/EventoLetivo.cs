using System.ComponentModel.DataAnnotations;

namespace SME.SR.Infra
{
    public enum EventoLetivo
    {
        [Display(Name = "Sim")]
        Sim = 1,

        [Display(Name = "Não")]
        Nao = 2,

        [Display(Name = "Opcional")]
        Opcional = 3
    }
}
