using MediatR;
using SME.SR.Infra;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application.Queries.RelatorioFaltasFrequencia
{
    public class ObterRelatorioFaltasFrequenciaQueryHandler : IRequestHandler<ObterRelatorioFaltasFrequenciaQuery, RelatorioFaltasFrequenciaDto>
    {
        public async Task<RelatorioFaltasFrequenciaDto> Handle(ObterRelatorioFaltasFrequenciaQuery request, CancellationToken cancellationToken)
        {
            var relatorioFaltasFrequenciaDto = new RelatorioFaltasFrequenciaDto();
            return await Task.FromResult(relatorioFaltasFrequenciaDto);
        }
    }
}
