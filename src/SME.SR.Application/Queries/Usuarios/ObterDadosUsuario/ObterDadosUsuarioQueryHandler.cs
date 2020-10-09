using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterDadosUsuarioQueryHandler : IRequestHandler<ObterDadosUsuarioQuery, Usuario>
    {
        private readonly IUsuarioRepository usuarioRepository;

        public ObterDadosUsuarioQueryHandler(IUsuarioRepository usuarioRepository)
        {
            this.usuarioRepository = usuarioRepository ?? throw new ArgumentNullException(nameof(usuarioRepository));
        }
        public async Task<Usuario> Handle(ObterDadosUsuarioQuery request, CancellationToken cancellationToken)
        {
            return await usuarioRepository.ObterDados(request.CodigoRf);
        }
    }
}
