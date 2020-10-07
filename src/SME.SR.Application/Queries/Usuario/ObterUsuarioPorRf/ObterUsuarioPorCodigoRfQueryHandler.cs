using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterUsuarioPorCodigoRfQueryHandler : IRequestHandler<ObterUsuarioPorCodigoRfQuery, Usuario>
    {

        private readonly IUsuarioRepository usuarioRepository;
        public ObterUsuarioPorCodigoRfQueryHandler(IUsuarioRepository usuarioRepository)
        {
            this.usuarioRepository = usuarioRepository ?? throw new ArgumentNullException(nameof(usuarioRepository));
        }

        public async Task<Usuario> Handle(ObterUsuarioPorCodigoRfQuery request, CancellationToken cancellationToken)
        {
            var usuario = await usuarioRepository.ObterPorCodigoRF(request.UsuarioRf);

            if (usuario == null)
                throw new NegocioException("Não foi possível encontrar o usuário");

            return usuario;
        }
    }
}
