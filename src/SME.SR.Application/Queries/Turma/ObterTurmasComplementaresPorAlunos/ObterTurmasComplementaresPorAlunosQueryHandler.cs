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
    public class ObterTurmasComplementaresPorAlunosQueryHandler : IRequestHandler<ObterTurmasComplementaresPorAlunosQuery, IEnumerable<Turma>>
    {
        private readonly ITurmaRepository turmaSgpRepository;
        public ObterTurmasComplementaresPorAlunosQueryHandler(ITurmaRepository turmaSgpRepository)
        {
            this.turmaSgpRepository = turmaSgpRepository ?? throw new ArgumentNullException(nameof(turmaSgpRepository));
        }
        public async Task<IEnumerable<Turma>> Handle(ObterTurmasComplementaresPorAlunosQuery request, CancellationToken cancellationToken)
        {
            return await turmaSgpRepository.ObterTurmasComplementaresPorAlunos(request.AlunosCodigos);
        }
    }
}
