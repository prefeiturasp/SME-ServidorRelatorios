using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
   public class ObterPerguntasPorGrupoQuery : IRequest<IEnumerable<PerguntasOrdemGrupoAutoralDto>>
    {
        public GrupoSondagemEnum GrupoSondagem  { get; set; }

        public ObterPerguntasPorGrupoQuery(GrupoSondagemEnum grupoSondagem, ComponenteCurricularSondagemEnum componenteCurricular)
        {
            GrupoSondagem = grupoSondagem;
            ComponenteCurricular = componenteCurricular;
        }

        public ComponenteCurricularSondagemEnum ComponenteCurricular { get; set; }
    }
}
