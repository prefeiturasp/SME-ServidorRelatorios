using MediatR;
using SME.SR.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterNomeUsuarioPorLoginQueryHandler : IRequestHandler<ObterNomeUsuarioPorLoginQuery, string>
    {
        private readonly IUsuarioRepository usuarioRepository;

        public ObterNomeUsuarioPorLoginQueryHandler(IUsuarioRepository usuarioRepository)
        {
            this.usuarioRepository = usuarioRepository ?? throw new ArgumentNullException(nameof(usuarioRepository));
        }

        public Task<string> Handle(ObterNomeUsuarioPorLoginQuery request, CancellationToken cancellationToken)
                => usuarioRepository.ObterNomeUsuarioPorLogin(request.Login);
    }
}
