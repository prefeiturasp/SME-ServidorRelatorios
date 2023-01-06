using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterEncaminhamentosAEEQueryHandler : IRequestHandler<ObterEncaminhamentosAEEQuery, IEnumerable<EncaminhamentoAeeDto>>
    {
        private readonly IEncaminhamentoAeeRepository encaminhamentoAeeRepository;

        public ObterEncaminhamentosAEEQueryHandler(IEncaminhamentoAeeRepository encaminhamentoAeeRepository)
        {
            this.encaminhamentoAeeRepository = encaminhamentoAeeRepository ?? throw new ArgumentNullException(nameof(encaminhamentoAeeRepository));
        }

        public Task<IEnumerable<EncaminhamentoAeeDto>> Handle(ObterEncaminhamentosAEEQuery request, CancellationToken cancellationToken)
        {
            return encaminhamentoAeeRepository.ObterEncaminhamentosAEE(request.Filtro);   
        }
    }
}
