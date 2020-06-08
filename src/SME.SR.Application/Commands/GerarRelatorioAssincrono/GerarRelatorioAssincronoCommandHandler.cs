using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class GerarRelatorioAssincronoCommandHandler : IRequestHandler<GerarRelatorioAssincronoCommand, bool>
    {
        public Task<bool> Handle(GerarRelatorioAssincronoCommand request, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }
    }
}
