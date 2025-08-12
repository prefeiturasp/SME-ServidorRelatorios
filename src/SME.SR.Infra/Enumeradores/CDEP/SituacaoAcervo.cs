using System.ComponentModel.DataAnnotations;

namespace SME.SR.Infra.CDEP
{
    public enum SituacaoAcervo
    {
        [Display(Description = "Ativo")]
        ATIVO = 1,

        [Display(Description = "Inativo")]
        INATIVO = 2,
    }
}
