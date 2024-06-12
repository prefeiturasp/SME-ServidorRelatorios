using System.ComponentModel.DataAnnotations;

namespace SME.SR.Data.Models.Conecta
{
    public enum Formato
    {
        [Display(Name = "Presencial")]
        Presencial = 1,
        [Display(Name = "A Distância")]
        Distancia = 2,
        [Display(Name = "Híbrido")]
        Hibrido = 3
    }
}
