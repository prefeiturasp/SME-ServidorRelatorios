using System.ComponentModel.DataAnnotations;

namespace SME.SR.Workers.SGP.Infra
{
    public enum ModalidadeTipoCalendario
    {
        [Display(Name = "Fundamental/Médio")]
        FundamentalMedio = 1,

        [Display(Name = "EJA")]
        EJA = 2
    }
}
