using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application.Queries
{
    public class ObterAlunosTurmaPorTurmaDataSituacaoMatriculaQueryHandler : IRequestHandler<ObterAlunosTurmaPorTurmaDataSituacaoMatriculaQuery, IEnumerable<Aluno>>
    {
        private readonly ITurmaRepository turmaRepository;

        public ObterAlunosTurmaPorTurmaDataSituacaoMatriculaQueryHandler(ITurmaRepository turmaRepository)
        {
            this.turmaRepository = turmaRepository ?? throw new ArgumentNullException(nameof(turmaRepository));
        }
        public async Task<IEnumerable<Aluno>> Handle(ObterAlunosTurmaPorTurmaDataSituacaoMatriculaQuery request, CancellationToken cancellationToken)
        {
            return await turmaRepository.ObterDadosAlunosPorTurmaDataMatricula(request.TurmaCodigo, request.Referencia);
        }
    }
}
