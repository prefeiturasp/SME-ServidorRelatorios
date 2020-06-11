using MediatR;
using SME.SR.Data.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterParametroSistemaPorTipoQueryHandler : IRequestHandler<ObterParametroSistemaPorTipoQuery, string>
    {
        private IParametroSistemaRepository _parametroSistemaRepository;

        public ObterParametroSistemaPorTipoQueryHandler(IParametroSistemaRepository parametroSistemaRepository)
        {
            this._parametroSistemaRepository = parametroSistemaRepository;
        }

        public async Task<string> Handle(ObterParametroSistemaPorTipoQuery request, CancellationToken cancellationToken)
        {
            return await _parametroSistemaRepository.ObterValorPorTipo(request.TipoParametro);
        }
    }
}
