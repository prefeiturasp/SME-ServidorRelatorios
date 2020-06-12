using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
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
            return await _turmaRepository.ObterDadosAlunos(request.TurmaCodigo);
        }
    }
}
