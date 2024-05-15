using System.ComponentModel.DataAnnotations;

namespace SME.SR.Data.Models.Conecta
{
    public enum TipoFormacao
    {
        [Display(Name = "Curso")]
        Curso = 1,
        [Display(Name = "Evento")]
        Evento = 2
    }
}
