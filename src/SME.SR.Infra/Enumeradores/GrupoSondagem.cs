using System.ComponentModel.DataAnnotations;

namespace SME.SR.Infra
{
    public enum GrupoSondagem
    {
        [Display(Name = "LP - Escrita")]
        LP_Escrita = 1,
        [Display(Name = "LP - Leitura")]
        LP_Leitura = 2,
        [Display(Name = "Leitura em voz alta")]
        LP_LeituraVozAlta = "",
        [Display(Name = "LP - Capacidade de Leitura")]
        LP_CapacidadeLeitura = 4,
        [Display(Name = "LP - Produção de texto")]
        LP_ProducaoTexto = 5
    }
}