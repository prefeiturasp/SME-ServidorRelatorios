using MediatR;
using SME.SR.Workers.SGP.Commons;
using SME.SR.Workers.SGP.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Workers.SGP.Queries
{
    public class ObterAlunosPorTurmaQueryHandler : IRequestHandler<ObterAlunosPorTurmaQuery, IEnumerable<Aluno>>
    {
        private ITurmaRepository _turmaRepository;

        public ObterAlunosPorTurmaQueryHandler(ITurmaRepository turmaRepository)
        {
            this._turmaRepository = turmaRepository;
        }

        public async Task<IEnumerable<Aluno>> Handle(ObterAlunosPorTurmaQuery request, CancellationToken cancellationToken)
        {
            return await _turmaRepository.ObterDadosAlunos(request.CodigoTurma);
        }
    }
}
