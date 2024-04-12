using System.ComponentModel.DataAnnotations;

namespace SME.SR.Infra
{
    public enum GrupoSondagemEnum
    {
        [Display(Name = "6a3d323a-2c44-4052-ba68-13a8dead299a", ShortName = "IAD - Leitura em voz alta")]
        LeituraVozAlta = 1,
        [Display(Name = "263b55b8-efa2-480c-80ad-f4e8f0935e12", ShortName = "IAD - Produção de texto")]
        ProducaoTexto = 2,
        [Display(Name = "e27b99a3-789d-43fb-a962-7df8793622b1", ShortName = "IAD - Capacidade de leitura")]
        CapacidadeLeitura = 3,
    }
}
