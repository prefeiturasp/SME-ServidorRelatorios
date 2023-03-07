using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterEncaminhamentosNAAPAQueryHandler : IRequestHandler<ObterEncaminhamentosNAAPAQuery, IEnumerable<EncaminhamentoNAAPADto>>
    {
        private readonly IEncaminhamentoNAAPARepository encaminhamentoAeeRepository;

        public ObterEncaminhamentosNAAPAQueryHandler(IEncaminhamentoNAAPARepository encaminhamentoAeeRepository)
        {
            this.encaminhamentoAeeRepository = encaminhamentoAeeRepository ?? throw new ArgumentNullException(nameof(encaminhamentoAeeRepository));
        }

        public Task<IEnumerable<EncaminhamentoNAAPADto>> Handle(ObterEncaminhamentosNAAPAQuery request, CancellationToken cancellationToken)
        {
            return encaminhamentoAeeRepository.ObterEncaminhamentosNAAPAPorIds(request.filtroRelatorioEncaminhamentoNaapaDetalhadoDto.EncaminhamentoNaapaIds);
        }
    }
}
