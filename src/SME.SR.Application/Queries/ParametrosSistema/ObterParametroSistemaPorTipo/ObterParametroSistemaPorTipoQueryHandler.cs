using MediatR;
using SME.SR.Data.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterParametroSistemaPorTipoQueryHandler : IRequestHandler<ObterParametroSistemaPorTipoQuery, string>
    {
        private readonly IParametroSistemaRepository parametroSistemaRepository;

        public ObterParametroSistemaPorTipoQueryHandler(IParametroSistemaRepository parametroSistemaRepository)
        {
            this.parametroSistemaRepository = parametroSistemaRepository;
        }

        public async Task<string> Handle(ObterParametroSistemaPorTipoQuery request, CancellationToken cancellationToken)
        {
            return await parametroSistemaRepository.ObterValorPorTipo(request.TipoParametro);
        }
    }
}
