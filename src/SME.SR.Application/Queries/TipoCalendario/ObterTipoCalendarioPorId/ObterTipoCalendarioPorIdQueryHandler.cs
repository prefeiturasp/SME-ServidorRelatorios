using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;

namespace SME.SR.Application
{
    public class ObterTipoCalendarioPorIdQueryHandler : IRequestHandler<ObterTipoCalendarioPorIdQuery, TipoCalendarioDto>
    {
        private readonly ITipoCalendarioRepository tipoCalendariORepository;

        public ObterTipoCalendarioPorIdQueryHandler(ITipoCalendarioRepository tipoCalendariORepository)
        {
            this.tipoCalendariORepository = tipoCalendariORepository ?? throw new System.ArgumentNullException(nameof(tipoCalendariORepository));
        }
        public async Task<TipoCalendarioDto> Handle(ObterTipoCalendarioPorIdQuery request, CancellationToken cancellationToken)
        {
            return await tipoCalendariORepository.ObterPorId(request.Id);
        }
    }
}

