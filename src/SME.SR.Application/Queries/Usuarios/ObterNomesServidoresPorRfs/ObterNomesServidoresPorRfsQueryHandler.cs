using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterNomesServidoresPorRfsQueryHandler : IRequestHandler<ObterNomesServidoresPorRfsQuery, IEnumerable<Funcionario>>
    {
        private readonly IFuncionarioRepository funcionarioRepository;

        public ObterNomesServidoresPorRfsQueryHandler(IFuncionarioRepository funcionarioRepository)
        {
            this.funcionarioRepository = funcionarioRepository ?? throw new ArgumentNullException(nameof(funcionarioRepository));
        }

        public Task<IEnumerable<Funcionario>> Handle(ObterNomesServidoresPorRfsQuery request, CancellationToken cancellationToken)
        {
            return funcionarioRepository.ObterNomesServidoresPorRfs(request.CodigosRfs);
        }
    }
}
