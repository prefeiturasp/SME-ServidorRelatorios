using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterHistoricoNotasFechamentoPorTurmaIdQueryHandler : IRequestHandler<ObterHistoricoNotasFechamentoPorTurmaIdQuery, IEnumerable<HistoricoAlteracaoNotasFechamentoDto>>
    {
        private readonly IFechamentoNotaRepository fechamentoNotaRepository;

        public ObterHistoricoNotasFechamentoPorTurmaIdQueryHandler(IFechamentoNotaRepository fechamentoNotaRepository)
        {
            this.fechamentoNotaRepository = fechamentoNotaRepository ?? throw new ArgumentNullException(nameof(fechamentoNotaRepository));
        }

        public Task<IEnumerable<HistoricoAlteracaoNotasFechamentoDto>> Handle(ObterHistoricoNotasFechamentoPorTurmaIdQuery request, CancellationToken cancellationToken)
                => fechamentoNotaRepository.ObterHistoricoAlteracaoNotasFechamento(request.TurmaId);
    }
}
