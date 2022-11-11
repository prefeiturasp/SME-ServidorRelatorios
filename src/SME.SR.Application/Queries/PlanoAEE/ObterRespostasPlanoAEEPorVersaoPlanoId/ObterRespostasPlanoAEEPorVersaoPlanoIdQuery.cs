using System.Collections.Generic;
using MediatR;
using SME.SR.Infra;

namespace SME.SR.Application
{
    public class ObterRespostasPlanoAEEPorVersaoPlanoIdQuery : IRequest<IEnumerable<RespostaQuestaoDto>>
    {
        public ObterRespostasPlanoAEEPorVersaoPlanoIdQuery(long versaoPlanoId)
        {
            VersaoPlanoId = versaoPlanoId;
        }

        public long VersaoPlanoId { get; }
    }
}