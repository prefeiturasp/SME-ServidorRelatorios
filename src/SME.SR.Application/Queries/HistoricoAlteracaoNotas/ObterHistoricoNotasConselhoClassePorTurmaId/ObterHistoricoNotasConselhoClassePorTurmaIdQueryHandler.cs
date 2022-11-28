using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterHistoricoNotasConselhoClassePorTurmaIdQueryHandler : IRequestHandler<ObterHistoricoNotasConselhoClassePorTurmaIdQuery, IEnumerable<HistoricoAlteracaoNotasDto>>
    {
        private readonly IConselhoClasseNotaRepository conselhoClasseNotaRepository;
        
        public ObterHistoricoNotasConselhoClassePorTurmaIdQueryHandler(IConselhoClasseNotaRepository conselhoClasseNotaRepository)
        {
            this.conselhoClasseNotaRepository = conselhoClasseNotaRepository ?? throw new ArgumentNullException(nameof(conselhoClasseNotaRepository));
        }

        public async Task<IEnumerable<HistoricoAlteracaoNotasDto>> Handle(ObterHistoricoNotasConselhoClassePorTurmaIdQuery request, CancellationToken cancellationToken)
                => await conselhoClasseNotaRepository.ObterHistoricoAlteracaoNotasConselhoClasse(request.TurmaId, request.TipoCalendarioId, request.Bimestres, request.Componentes);
    }
}
