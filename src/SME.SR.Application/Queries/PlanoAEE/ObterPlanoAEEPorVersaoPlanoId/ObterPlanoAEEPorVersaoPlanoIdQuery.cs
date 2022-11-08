using MediatR;
using SME.SR.Infra;

namespace SME.SR.Application
{
    public class ObterPlanoAEEPorVersaoPlanoIdQuery : IRequest<PlanoAeeDto>
    {
        public ObterPlanoAEEPorVersaoPlanoIdQuery(long versaoPlanoId)
        {
            VersaoPlanoId = versaoPlanoId;
        }

        public long VersaoPlanoId { get; }
    }
}