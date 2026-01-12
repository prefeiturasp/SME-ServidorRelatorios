using System.ComponentModel.DataAnnotations;

namespace SME.SR.Infra.CDEP
{
    public enum SituacaoSolicitacaoItem
    {
        [Display(Description = "Aguardando atendimento")]
        AGUARDANDO_ATENDIMENTO = 1,

        [Display(Description = "Aguardando visita")]
        AGUARDANDO_VISITA = 2,

        [Display(Description = "Finalizado automaticamente")]
        FINALIZADO_AUTOMATICAMENTE = 3,

        [Display(Description = "Cancelado")]
        CANCELADO = 4,

        [Display(Description = "Finalizado manualmente")]
        FINALIZADO_MANUALMENTE = 5,

        [Display(Description = "Sem resposta do solicitante")]
        SEM_RESPOSTA_SOLICITANTE = 6
    }
}
