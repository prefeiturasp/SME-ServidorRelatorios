using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterNomesUsuariosPorRfsQueryHandler : IRequestHandler<ObterNomesUsuariosPorRfsQuery, IEnumerable<Usuario>>
    {
        private readonly IUsuarioRepository usuarioRepository;

        public ObterNomesUsuariosPorRfsQueryHandler(IUsuarioRepository usuarioRepository)
        {
            this.usuarioRepository = usuarioRepository ?? throw new ArgumentNullException(nameof(usuarioRepository));
        }

        public Task<IEnumerable<Usuario>> Handle(ObterNomesUsuariosPorRfsQuery request, CancellationToken cancellationToken)
        {
            return usuarioRepository.ObterNomesUsuariosPorRfs(request.CodigosRfs);
        }
    }
}
