using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterSituacaoUsuarioPorRfQueryHandler : IRequestHandler<ObterSituacaoUsuarioPorRfQuery, SituacaoUsuario>
    {
        private readonly IUsuarioRepository usuarioRepository;

        public ObterSituacaoUsuarioPorRfQueryHandler(IUsuarioRepository usuarioRepository)
        {
            this.usuarioRepository = usuarioRepository ?? throw new ArgumentNullException(nameof(usuarioRepository));
        }

        public async Task<SituacaoUsuario> Handle(ObterSituacaoUsuarioPorRfQuery request, CancellationToken cancellationToken)
            => await usuarioRepository.ObterSituacaoUsuarioPorRf(request.UsuarioRf);
    }
}
