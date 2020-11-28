using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterHistoricoNotasConselhoClasseQueryHandler : IRequestHandler<ObterHistoricoNotasFechamentoPorTurmaIdQuery, IEnumerable<HistoricoAlteracaoNotasDto>>
    {
        private readonly IConselhoClasseRepository conselhoClasseRepository;

        public ObterHistoricoNotasConselhoClasseQueryHandler(IConselhoClasseRepository conselhoClasseRepository)
        {
            this.conselhoClasseRepository = conselhoClasseRepository ?? throw new ArgumentNullException(nameof(conselhoClasseRepository));
        }

        public async Task<IEnumerable<HistoricoAlteracaoNotasDto>> Handle(ObterHistoricoNotasFechamentoPorTurmaIdQuery request, CancellationToken cancellationToken)
                => await conselhoClasseRepository.ObterHistoricoAlteracaoNotasConselhoClasse(request.TurmaId);
    }
}
