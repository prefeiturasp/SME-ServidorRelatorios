using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterDadosUsuarioCoreSSOPorRfQueryHandler : IRequestHandler<ObterDadosUsuarioCoreSSOPorRfQuery, UsuarioCoreSSO>
    {
        private readonly IUsuarioRepository usuarioRepository;

        public ObterDadosUsuarioCoreSSOPorRfQueryHandler(IUsuarioRepository funcionarioRepository)
        {
            this.usuarioRepository = funcionarioRepository ?? throw new ArgumentNullException(nameof(funcionarioRepository));
        }

        public async Task<UsuarioCoreSSO> Handle(ObterDadosUsuarioCoreSSOPorRfQuery request, CancellationToken cancellationToken)
            => await usuarioRepository.ObterDadosCoreSSO(request.UsuarioRf);
    }
}
