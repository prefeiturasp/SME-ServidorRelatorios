using System.ComponentModel.DataAnnotations;

namespace SME.SR.Infra
{
    public enum TipoEncontroConecta
    {
        [Display(Name = "Presencial")]
        Presencial,
        [Display(Name = "Síncrono")]
        Sincrono,
        [Display(Name = "Assíncrono")]
        Assincrono
    }
}
