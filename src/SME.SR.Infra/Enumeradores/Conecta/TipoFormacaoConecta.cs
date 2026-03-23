using System.ComponentModel.DataAnnotations;

namespace SME.SR.Infra
{
    public enum TipoFormacaoConecta
    {
        [Display(Name = "Curso")]
        Curso = 1,
        [Display(Name = "Evento")]
        Evento = 2
    }
}
