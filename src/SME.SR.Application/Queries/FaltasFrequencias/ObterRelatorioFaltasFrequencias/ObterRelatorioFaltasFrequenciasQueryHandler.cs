using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterRelatorioFaltasFrequenciasQueryHandler : IRequestHandler<ObterRelatorioFaltasFrequenciasQuery, RelatorioFaltasFrequenciasDto>
    {


        public ObterRelatorioFaltasFrequenciasQueryHandler()
        {

        }

        public async Task<RelatorioFaltasFrequenciasDto> Handle(ObterRelatorioFaltasFrequenciasQuery request, CancellationToken cancellationToken)
        {
            return await Task.FromResult(new RelatorioFaltasFrequenciasDto());
        }


    }
}
