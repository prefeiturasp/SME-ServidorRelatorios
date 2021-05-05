using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterAlunosReduzidosPorTurmaEAlunoQueryHandler : IRequestHandler<ObterAlunosReduzidosPorTurmaEAlunoQuery, IEnumerable<AlunoReduzidoDto>>
    {
        private readonly IAlunoRepository alunoRepository;

        public ObterAlunosReduzidosPorTurmaEAlunoQueryHandler(IAlunoRepository alunoRepository)
        {
            this.alunoRepository = alunoRepository ?? throw new ArgumentNullException(nameof(alunoRepository));
        }

        public async Task<IEnumerable<AlunoReduzidoDto>> Handle(ObterAlunosReduzidosPorTurmaEAlunoQuery request, CancellationToken cancellationToken)
        {
            var alunos = await alunoRepository.ObterAlunosReduzidosPorTurmaEAluno(long.Parse(request.TurmaCodigo), request.AlunoCodigo);

            return alunos;
        }
    }
}
