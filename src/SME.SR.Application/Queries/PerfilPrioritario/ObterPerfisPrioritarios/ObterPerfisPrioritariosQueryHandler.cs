using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterPerfisPrioritariosQueryHandler : IRequestHandler<ObterPerfisPrioritariosQuery, IEnumerable<PrioridadePerfil>>
    {
        private readonly IUsuarioRepository usuarioRepository;

        public ObterPerfisPrioritariosQueryHandler(IUsuarioRepository usuarioRepository)
        {
            this.usuarioRepository = usuarioRepository ?? throw new ArgumentNullException(nameof(usuarioRepository));
        }

        public async Task<IEnumerable<PrioridadePerfil>> Handle(ObterPerfisPrioritariosQuery request, CancellationToken cancellationToken)
            => await usuarioRepository.ObterListaPrioridadePerfil();
    }
}
