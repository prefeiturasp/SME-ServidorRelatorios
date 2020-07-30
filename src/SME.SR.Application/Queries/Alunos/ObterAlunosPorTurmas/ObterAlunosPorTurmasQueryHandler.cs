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
    public class ObterAlunosPorTurmasQueryHandler : IRequestHandler<ObterAlunosPorTurmasQuery, IEnumerable<Aluno>>
    {
        private readonly ITurmaRepository turmaRepository;

        public ObterAlunosPorTurmasQueryHandler(ITurmaRepository turmaRepository)
        {
            this.turmaRepository = turmaRepository ?? throw new ArgumentNullException(nameof(turmaRepository));
        }

        public async Task<IEnumerable<Aluno>> Handle(ObterAlunosPorTurmasQuery request, CancellationToken cancellationToken)
        {
            return await turmaRepository.ObterAlunosPorTurmas(request.TurmasId);
        }
    }
}
