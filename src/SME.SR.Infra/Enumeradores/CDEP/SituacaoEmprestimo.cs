using System.ComponentModel.DataAnnotations;

namespace SME.SR.Infra.CDEP
{
    public enum SituacaoEmprestimo
    {
        [Display(Description = "Emprestado")]
        EMPRESTADO = 1,

        [Display(Description = "Devolução em atraso")]
        DEVOLUCAO_EM_ATRASO = 2,

        [Display(Description = "Emprestado - Prorrogação")]
        EMPRESTADO_PRORROGACAO = 3,

        [Display(Description = "Devolvido")]
        DEVOLVIDO = 4
    }
}
