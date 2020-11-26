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
    public class UsuarioPossuiSenhaPadraoQueryHandler : IRequestHandler<UsuarioPossuiSenhaPadraoQuery, bool>
    {
        private readonly IUsuarioRepository usuarioRepository;

        public UsuarioPossuiSenhaPadraoQueryHandler(IUsuarioRepository usuarioRepository)
        {
            this.usuarioRepository = usuarioRepository ?? throw new ArgumentNullException(nameof(usuarioRepository));
        }


        public async Task<bool> Handle(UsuarioPossuiSenhaPadraoQuery request, CancellationToken cancellationToken)
        {
            var rf = request.UsuarioRf;
            var senhaPadrao = $"Sgp{rf.Substring(rf.Length - 4)}";

            var usuario = await usuarioRepository.ObterDadosCoreSSO(rf);
            return senhaPadrao.CriptografarSenha(usuario.TipoCriptografia).Equals(usuario.Senha);
        }
    }
}
