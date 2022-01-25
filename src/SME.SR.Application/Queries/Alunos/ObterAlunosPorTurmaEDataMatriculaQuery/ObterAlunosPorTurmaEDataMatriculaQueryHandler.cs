using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterAlunosPorTurmaEDataMatriculaQueryHandler : IRequestHandler<ObterAlunosPorTurmaEDataMatriculaQuery, IEnumerable<AlunoPorTurmaRespostaDto>>
    {
        private readonly IAlunoRepository alunoRepository;
        public ObterAlunosPorTurmaEDataMatriculaQueryHandler(IAlunoRepository alunoRepository)
        {
            this.alunoRepository = alunoRepository ?? throw new ArgumentNullException(nameof(alunoRepository));
        }

        public async Task<IEnumerable<AlunoPorTurmaRespostaDto>> Handle(ObterAlunosPorTurmaEDataMatriculaQuery request, CancellationToken cancellationToken)
        {
            return await alunoRepository.ObterAlunosPorTurmaEDataMatriculaQuery(long.Parse(request.CodigoTurma), request.DataMatricula);
        }
    }
}
