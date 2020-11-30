using System.ComponentModel.DataAnnotations;

namespace SME.SR.Infra
{
    public enum TipoAlteracaoNota
    {
        [Display(Name = "Ambas")]
        Ambas = 1,

        [Display(Name = "Fechamento")]
        Fechamento = 2,

        [Display(Name = "Conselho de Classe")]
        ConselhoClasse = 3
    }
}
