using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterEncaminhamentosAEEPorIdQueryHandler : IRequestHandler<ObterEncaminhamentosAEEPorIdQuery, IEnumerable<EncaminhamentoAeeDto>>
    {
        private readonly IEncaminhamentoAeeRepository encaminhamentoAeeRepository;

        public ObterEncaminhamentosAEEPorIdQueryHandler(IEncaminhamentoAeeRepository encaminhamentoAeeRepository)
        {
            this.encaminhamentoAeeRepository = encaminhamentoAeeRepository ?? throw new ArgumentNullException(nameof(encaminhamentoAeeRepository));
        }

        public Task<IEnumerable<EncaminhamentoAeeDto>> Handle(ObterEncaminhamentosAEEPorIdQuery request, CancellationToken cancellationToken)
        {
            return encaminhamentoAeeRepository.ObterEncaminhamentosAEEPorIds(request.Filtro.Ids);   
        }
    }
}
