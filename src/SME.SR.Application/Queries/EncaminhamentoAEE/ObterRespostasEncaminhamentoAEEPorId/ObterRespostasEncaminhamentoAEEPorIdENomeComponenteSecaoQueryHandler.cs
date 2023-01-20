using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;

namespace SME.SR.Application
{
    public class ObterRespostasEncaminhamentoAEEPorIdENomeComponenteSecaoQueryHandler : IRequestHandler<ObterRespostasEncaminhamentoAEEPorIdENomeComponenteSecaoQuery, IEnumerable<RespostaQuestaoDto>>
    {
        private readonly IEncaminhamentoAeeRespostaRepository encaminhamentoAeeRespostaRepository;

        public ObterRespostasEncaminhamentoAEEPorIdENomeComponenteSecaoQueryHandler(IEncaminhamentoAeeRespostaRepository encaminhamentoAeeRespostaRepository)
        {
            this.encaminhamentoAeeRespostaRepository = encaminhamentoAeeRespostaRepository ?? throw new ArgumentNullException(nameof(encaminhamentoAeeRespostaRepository));
        }

        public async Task<IEnumerable<RespostaQuestaoDto>> Handle(ObterRespostasEncaminhamentoAEEPorIdENomeComponenteSecaoQuery request, CancellationToken cancellationToken)
        {
            return await encaminhamentoAeeRespostaRepository.ObterRespostasPorEncaminhamentoId(request.EncaminhamentoAeeId, request.NomeComponenteSecao);
        }
    }
}