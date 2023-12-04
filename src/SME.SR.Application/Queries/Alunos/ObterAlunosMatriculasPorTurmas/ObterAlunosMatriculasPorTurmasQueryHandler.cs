using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Data.Interfaces.ElasticSearch;
using SME.SR.Infra;
using SME.SR.Infra.Dtos.ElasticSearch;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterAlunosMatriculasPorTurmasQueryHandler : IRequestHandler<ObterAlunosMatriculasPorTurmasQuery, IEnumerable<AlunoNaTurmaDTO>>
    {
        private readonly IRepositorioElasticTurma repositorioElasticTurma;

        public ObterAlunosMatriculasPorTurmasQueryHandler(IRepositorioElasticTurma repositorioElasticTurma)
        {
            this.repositorioElasticTurma = repositorioElasticTurma;
        }

        public async Task<IEnumerable<AlunoNaTurmaDTO>> Handle(ObterAlunosMatriculasPorTurmasQuery request, CancellationToken cancellationToken)
        {
            return await this.repositorioElasticTurma
                .ObterMatriculasAlunoNaTurma(request.CodigosTurmas);
        }
    }
}
