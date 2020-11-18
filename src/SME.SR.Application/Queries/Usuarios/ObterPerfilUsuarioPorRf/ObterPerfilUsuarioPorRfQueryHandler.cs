using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterPerfilUsuarioPorRfQueryHandler : IRequestHandler<ObterPerfilUsuarioPorRfQuery, IEnumerable<PerfilUsuarioDto>>
    {
        private readonly IFuncionarioRepository funcionarioRepository;

        public ObterPerfilUsuarioPorRfQueryHandler(IFuncionarioRepository funcionarioRepository)
        {
            this.funcionarioRepository = funcionarioRepository ?? throw new ArgumentNullException(nameof(funcionarioRepository));
        }

        public async Task<IEnumerable<PerfilUsuarioDto>> Handle(ObterPerfilUsuarioPorRfQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
