using MediatR;
using SME.SR.Workers.SGP.Commons;
using SME.SR.Workers.SGP.Models;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Workers.SGP.Queries
{
    public class ObterDadosAlunoQueryHandler : IRequestHandler<ObterDadosAlunoQuery, Aluno>
    {
        private IAlunoRepository _alunoRepository;

        public ObterDadosAlunoQueryHandler(IAlunoRepository turmaEolRepository)
        {
            this._alunoRepository = turmaEolRepository;
        }

        public async Task<Aluno> Handle(ObterDadosAlunoQuery request, CancellationToken cancellationToken)
        {
            return await _alunoRepository.ObterDados(request.CodigoTurma, request.CodigoAluno);
        }
    }
}
