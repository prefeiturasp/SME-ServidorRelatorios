using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterPerfisUsuarioPorRfQueryHandler : IRequestHandler<ObterPerfisUsuarioPorRfQuery, IEnumerable<Guid>>
    {
        private readonly IFuncionarioRepository funcionarioRepository;

        public ObterPerfisUsuarioPorRfQueryHandler(IFuncionarioRepository funcionarioRepository)
        {
            this.funcionarioRepository = funcionarioRepository ?? throw new ArgumentNullException(nameof(funcionarioRepository));
        }

        public async Task<IEnumerable<Guid>> Handle(ObterPerfisUsuarioPorRfQuery request, CancellationToken cancellationToken)
            => await funcionarioRepository.ObterPerfisUsuarioPorRf(request.UsuarioRf);
    }
}
