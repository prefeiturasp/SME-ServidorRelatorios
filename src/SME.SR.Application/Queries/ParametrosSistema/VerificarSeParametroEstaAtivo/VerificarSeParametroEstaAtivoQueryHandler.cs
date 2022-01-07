using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class VerificarSeParametroEstaAtivoQueryHandler : IRequestHandler<VerificarSeParametroEstaAtivoQuery, ParametroSistemaAnoSituacaoDto>
    {
        private readonly IParametroSistemaRepository parametroSistemaRepository;
        public VerificarSeParametroEstaAtivoQueryHandler(IParametroSistemaRepository parametroSistemaRepository)
        {
            this.parametroSistemaRepository = parametroSistemaRepository ?? throw new ArgumentNullException(nameof(parametroSistemaRepository));
        }

        public async Task<ParametroSistemaAnoSituacaoDto> Handle(VerificarSeParametroEstaAtivoQuery request, CancellationToken cancellationToken)
        {
            return await parametroSistemaRepository.VerificarSeParametroEstaAtivo(request.TipoParametro);
        }
    }
}
