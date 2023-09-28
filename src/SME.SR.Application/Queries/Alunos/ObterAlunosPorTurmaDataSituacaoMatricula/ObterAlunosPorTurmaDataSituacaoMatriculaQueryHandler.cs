using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application.Queries
{
    public class ObterAlunosPorTurmaDataSituacaoMatriculaQueryHandler : IRequestHandler<ObterAlunosPorTurmaDataSituacaoMatriculaQuery, IEnumerable<Aluno>>
    {
        private readonly IAlunoRepository alunoRepository;

        public ObterAlunosPorTurmaDataSituacaoMatriculaQueryHandler(IAlunoRepository alunoRepository)
        {
            this.alunoRepository = alunoRepository ?? throw new ArgumentNullException(nameof(alunoRepository));
        }
        public async Task<IEnumerable<Aluno>> Handle(ObterAlunosPorTurmaDataSituacaoMatriculaQuery request, CancellationToken cancellationToken)
        {
            return await alunoRepository.ObterAlunosPorTurmaDataSituacaoMatriculaParaSondagem(request.TurmaCodigo, request.DataReferenciaFim, request.DataReferenciaInicio);
        }
    }
}
