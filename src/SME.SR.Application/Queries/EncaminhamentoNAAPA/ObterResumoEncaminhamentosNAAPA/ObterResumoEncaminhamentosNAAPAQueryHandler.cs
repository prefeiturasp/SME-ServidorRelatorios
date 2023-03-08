using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterResumoEncaminhamentosNAAPAQueryHandler : IRequestHandler<ObterResumoEncaminhamentosNAAPAQuery, IEnumerable<EncaminhamentoNAAPASimplesDto>>
    {
        private readonly IEncaminhamentoNAAPARepository encaminhamentoAeeRepository;

        public ObterResumoEncaminhamentosNAAPAQueryHandler(IEncaminhamentoNAAPARepository encaminhamentoAeeRepository)
        {
            this.encaminhamentoAeeRepository = encaminhamentoAeeRepository ?? throw new ArgumentNullException(nameof(encaminhamentoAeeRepository));
        }

        public Task<IEnumerable<EncaminhamentoNAAPASimplesDto>> Handle(ObterResumoEncaminhamentosNAAPAQuery request, CancellationToken cancellationToken)
        {
            return encaminhamentoAeeRepository.ObterResumoEncaminhamentosNAAPA(request.Filtro);   
        }
    }
}
