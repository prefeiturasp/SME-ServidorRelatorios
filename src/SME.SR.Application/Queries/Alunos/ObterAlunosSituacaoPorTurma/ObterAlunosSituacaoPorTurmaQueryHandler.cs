using MediatR;
using SME.SR.Data.Interfaces.ElasticSearch;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterAlunosSituacaoPorTurmaQueryHandler : IRequestHandler<ObterAlunosSituacaoPorTurmaQuery, IEnumerable<AlunoSituacaoDto>>
    {
        private readonly IRepositorioElasticTurma repositorioElasticTurma;

        public ObterAlunosSituacaoPorTurmaQueryHandler(IRepositorioElasticTurma turmaRepository)
        {
            this.repositorioElasticTurma = turmaRepository ?? throw new ArgumentNullException(nameof(repositorioElasticTurma));
        }


        public async Task<IEnumerable<AlunoSituacaoDto>> Handle(ObterAlunosSituacaoPorTurmaQuery request, CancellationToken cancellationToken)
        {
            var alunos = await repositorioElasticTurma.ObterTodosAlunosNaTurmaAsync(Convert.ToInt32(request.TurmaCodigo));

            return alunos.GroupBy(a => a.CodigoAluno).SelectMany(x => x.OrderByDescending(y => y.DataSituacaoAluno).Take(1));
        }
    }
}
