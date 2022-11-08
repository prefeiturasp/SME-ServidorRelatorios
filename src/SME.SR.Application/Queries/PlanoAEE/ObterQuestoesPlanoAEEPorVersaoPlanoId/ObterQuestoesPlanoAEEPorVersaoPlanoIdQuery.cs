using System.Collections.Generic;
using MediatR;
using SME.SR.Infra;

namespace SME.SR.Application
{
    public class ObterQuestoesPlanoAEEPorVersaoPlanoIdQuery : IRequest<IEnumerable<QuestaoDto>>
    {
        public ObterQuestoesPlanoAEEPorVersaoPlanoIdQuery(long versaoPlanoId)
        {
            VersaoPlanoId = versaoPlanoId;
        }

        public long VersaoPlanoId { get; }
    }
}