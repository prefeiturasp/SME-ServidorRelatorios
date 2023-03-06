using System.ComponentModel.DataAnnotations;

namespace SME.SR.Infra
{
    public enum SituacaoNAAPA
    {
        [Display(Name = "Rascunho")]
        Rascunho = 1,
        [Display(Name = "Aguardando atendimento")]
        AguardandoAtendimento = 2,
        [Display(Name = "Em atendimento")]
        EmAtendimento = 3,
        [Display(Name = "Encerrado")]
        Encerrado = 4
    }
}
