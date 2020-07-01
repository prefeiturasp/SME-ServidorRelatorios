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
    public class ObterParecerConclusivoPorTurmaQueryHandler : IRequestHandler<ObterParecerConclusivoPorTurmaQuery, IEnumerable<ConselhoClasseParecerConclusivo>>
    {
        private IConselhoClasseAlunoRepository conselhoClasseAlunoRepository;

        public ObterParecerConclusivoPorTurmaQueryHandler(IConselhoClasseAlunoRepository conselhoClasseAlunoRepository)
        {
            this.conselhoClasseAlunoRepository = conselhoClasseAlunoRepository ?? throw new ArgumentNullException(nameof(conselhoClasseAlunoRepository));
        }

        public async Task<IEnumerable<ConselhoClasseParecerConclusivo>> Handle(ObterParecerConclusivoPorTurmaQuery request, CancellationToken cancellationToken)
            => await conselhoClasseAlunoRepository.ObterParecerConclusivoPorTurma(request.TurmaCodigo);
    }
}
