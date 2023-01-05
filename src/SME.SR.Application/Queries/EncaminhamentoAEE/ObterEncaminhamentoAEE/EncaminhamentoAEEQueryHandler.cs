using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class EncaminhamentoAEEQueryHandler : IRequestHandler<EncaminhamentoAEEQuery, IEnumerable<EncaminhamentoAeeDto>>
    {
        private readonly IEncaminhamentoAeeRepository encaminhamentoAeeRepository;

        public EncaminhamentoAEEQueryHandler(IEncaminhamentoAeeRepository encaminhamentoAeeRepository)
        {
            this.encaminhamentoAeeRepository = encaminhamentoAeeRepository ?? throw new ArgumentNullException(nameof(encaminhamentoAeeRepository));
        }

        public Task<IEnumerable<EncaminhamentoAeeDto>> Handle(EncaminhamentoAEEQuery request, CancellationToken cancellationToken)
        {
            return encaminhamentoAeeRepository.ObterEncaminhamentoAEE(request.Filtro);   
        }
    }
}
