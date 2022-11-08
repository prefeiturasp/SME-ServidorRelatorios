using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;

namespace SME.SR.Application
{
    public class ObterRespostasPlanoAEEPorVersaoPlanoIdQueryHandler : IRequestHandler<ObterRespostasPlanoAEEPorVersaoPlanoIdQuery, IEnumerable<RespostaQuestaoDto>>
    {
        private readonly IPlanoAeeRespostaRepository planoAeeRespostaRepository;

        public ObterRespostasPlanoAEEPorVersaoPlanoIdQueryHandler(IPlanoAeeRespostaRepository planoAeeRespostaRepository)
        {
            this.planoAeeRespostaRepository = planoAeeRespostaRepository ?? throw new ArgumentNullException(nameof(planoAeeRespostaRepository));
        }

        public async Task<IEnumerable<RespostaQuestaoDto>> Handle(ObterRespostasPlanoAEEPorVersaoPlanoIdQuery request, CancellationToken cancellationToken)
        {
            return await planoAeeRespostaRepository.ObterRespostasPorVersaoPlanoId(request.VersaoPlanoId);
        }
    }
}