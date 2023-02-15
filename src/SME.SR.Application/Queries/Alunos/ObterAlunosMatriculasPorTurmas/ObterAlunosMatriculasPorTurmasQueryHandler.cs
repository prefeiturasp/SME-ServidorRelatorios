using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterAlunosMatriculasPorTurmasQueryHandler : IRequestHandler<ObterAlunosMatriculasPorTurmasQuery, IEnumerable<AlunoTurma>>
    {
        private readonly IAlunoRepository alunoRepository;

        public ObterAlunosMatriculasPorTurmasQueryHandler(IAlunoRepository alunoRepository)
        {
            this.alunoRepository = alunoRepository ?? throw new ArgumentNullException(nameof(alunoRepository));
        }

        public async Task<IEnumerable<AlunoTurma>> Handle(ObterAlunosMatriculasPorTurmasQuery request, CancellationToken cancellationToken)
        {
            return await alunoRepository
                .ObterAlunosMatriculasPorTurmas(request.CodigosTurmas);
        }
    }
}
