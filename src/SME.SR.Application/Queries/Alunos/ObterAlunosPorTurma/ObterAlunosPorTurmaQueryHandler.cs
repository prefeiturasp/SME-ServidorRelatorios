using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterAlunosPorTurmaQueryHandler : IRequestHandler<ObterAlunosPorTurmaQuery, IEnumerable<Aluno>>
    {
        private readonly ITurmaRepository turmaRepository;

        public ObterAlunosPorTurmaQueryHandler(ITurmaRepository turmaRepository)
        {
            this.turmaRepository = turmaRepository ?? throw new ArgumentNullException(nameof(turmaRepository));
        }

        public async Task<IEnumerable<Aluno>> Handle(ObterAlunosPorTurmaQuery request, CancellationToken cancellationToken)
        {
            return await turmaRepository.ObterDadosAlunos(request.TurmaCodigo);
        }
    }
}
