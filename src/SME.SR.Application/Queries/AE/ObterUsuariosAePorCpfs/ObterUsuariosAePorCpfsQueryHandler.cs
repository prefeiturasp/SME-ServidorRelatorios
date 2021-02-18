using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterUsuariosAePorCpfsQueryHandler : IRequestHandler<ObterUsuariosAePorCpfsQuery, IEnumerable<UsuarioAEDto>>
    {
        private readonly IUsuarioAERepository usuarioAERepository;

        public ObterUsuariosAePorCpfsQueryHandler(IUsuarioAERepository usuarioAERepository)
        {
            this.usuarioAERepository = usuarioAERepository ?? throw new System.ArgumentNullException(nameof(usuarioAERepository));
        }
        public async Task<IEnumerable<UsuarioAEDto>> Handle(ObterUsuariosAePorCpfsQuery request, CancellationToken cancellationToken)
        {
            return await usuarioAERepository.ObterUsuarioAEPorCpfs(request.Cpfs);
        }
    }
}
