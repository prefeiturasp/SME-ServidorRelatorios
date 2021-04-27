using MediatR;
using SME.SR.Infra;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterDadosConsolidadosRegistroIndividualParaRelatorioQueryHandler : IRequestHandler<ObterDadosConsolidadosRegistroIndividualParaRelatorioQuery, RelatorioRegistroIndividualDto>
    {
        public Task<RelatorioRegistroIndividualDto> Handle(ObterDadosConsolidadosRegistroIndividualParaRelatorioQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
