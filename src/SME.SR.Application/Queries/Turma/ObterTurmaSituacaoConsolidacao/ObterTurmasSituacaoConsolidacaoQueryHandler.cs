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
    public class ObterTurmasSituacaoConsolidacaoQueryHandler : IRequestHandler<ObterTurmasSituacaoConsolidacaoQuery, IEnumerable<Turma>>
    {
        private readonly ITurmaRepository turmaRepository;

        public ObterTurmasSituacaoConsolidacaoQueryHandler(ITurmaRepository turmaRepository)
        {
            this.turmaRepository = turmaRepository ?? throw new ArgumentNullException(nameof(turmaRepository));
        }

        public async Task<IEnumerable<Turma>> Handle(ObterTurmasSituacaoConsolidacaoQuery request, CancellationToken cancellationToken)
                => await turmaRepository.ObterTurmasPorCodigosSituacaoConsolidado(request.Codigos, request.SituacaoFechamento, request.SituacaoConselhoClasse, request.Bimestres);
    }
}

