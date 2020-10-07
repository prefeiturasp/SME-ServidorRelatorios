using MediatR;
using SME.SR.Data;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterUsuarioPorCodigoRfQueryHandler : IRequestHandler<ObterUsuarioPorCodigoRfQuery, Usuario>
    {

        private readonly IPlanoAulaRepository planoAulaRepository;
        public ObterUsuarioPorCodigoRfQueryHandler(IPlanoAulaRepository planoAulaRepository)
        {
            this.planoAulaRepository = planoAulaRepository ?? throw new ArgumentNullException(nameof(planoAulaRepository));
        }

        public async Task<Usuario> Handle(ObterUsuarioPorCodigoRfQuery request, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}
