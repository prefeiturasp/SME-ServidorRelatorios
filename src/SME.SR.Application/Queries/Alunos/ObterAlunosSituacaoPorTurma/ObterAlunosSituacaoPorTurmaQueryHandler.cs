using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterAlunosSituacaoPorTurmaQueryHandler : IRequestHandler<ObterAlunosSituacaoPorTurmaQuery, IEnumerable<AlunoSituacaoDto>>
    {
        private ITurmaRepository turmaRepository;

        public ObterAlunosSituacaoPorTurmaQueryHandler(ITurmaRepository turmaRepository)
        {
            this.turmaRepository = turmaRepository ?? throw new ArgumentNullException(nameof(turmaRepository));
        }


        public async Task<IEnumerable<AlunoSituacaoDto>> Handle(ObterAlunosSituacaoPorTurmaQuery request, CancellationToken cancellationToken)
        {
            var alunos = await turmaRepository.ObterDadosAlunosSituacao(request.TurmaCodigo);

            return alunos.GroupBy(a => a.CodigoAluno).SelectMany(x => x.OrderBy(y => y.CodigoAluno).Take(1));
        }            
    }
}
