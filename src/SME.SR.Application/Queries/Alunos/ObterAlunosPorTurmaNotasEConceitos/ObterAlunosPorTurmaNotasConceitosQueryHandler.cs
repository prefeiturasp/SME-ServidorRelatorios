using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterAlunosPorTurmaNotasConceitosQueryHandler : IRequestHandler<ObterAlunosPorTurmaNotasConceitosQuery, IEnumerable<Aluno>>
    {
        private readonly ITurmaRepository turmaRepository;

        public ObterAlunosPorTurmaNotasConceitosQueryHandler(ITurmaRepository turmaRepository)
        {
            this.turmaRepository = turmaRepository ?? throw new ArgumentNullException(nameof(turmaRepository));
        }

        public async Task<IEnumerable<Aluno>> Handle(ObterAlunosPorTurmaNotasConceitosQuery request, CancellationToken cancellationToken)
        {
            return await turmaRepository.ObterDadosAlunosPorTurmaNotasConceitos(request.TurmaCodigo);
        }
    }
}
