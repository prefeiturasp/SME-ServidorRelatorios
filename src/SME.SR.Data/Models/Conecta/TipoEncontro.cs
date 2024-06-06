using System.ComponentModel.DataAnnotations;

namespace SME.SR.Data.Models.Conecta
{
    public enum TipoEncontro
    {
        [Display(Name = "Presencial")]
        Presencial,
        [Display(Name = "Síncrono")]
        Sincrono,
        [Display(Name = "Assíncrono")]
        Assincrono
    }
}
