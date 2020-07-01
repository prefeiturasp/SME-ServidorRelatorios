using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterAlunosTurmasRelatorioBoletimQueryHandler : IRequestHandler<ObterAlunosTurmasRelatorioBoletimQuery, IEnumerable<IGrouping<int, Aluno>>>
    {
        private IAlunoRepository alunoRepository;

        public ObterAlunosTurmasRelatorioBoletimQueryHandler(IAlunoRepository alunoRepository)
        {
            this.alunoRepository = alunoRepository;
        }

        public async Task<IEnumerable<IGrouping<int, Aluno>>> Handle(ObterAlunosTurmasRelatorioBoletimQuery request, CancellationToken cancellationToken)
        {
            var alunos = Enumerable.Empty<Aluno>();

            if (request.CodigosAlunos != null && request.CodigosAlunos.Length > 0)
                alunos = await alunoRepository.ObterPorCodigosAlunoETurma(request.CodigosTurma, request.CodigosAlunos);
            else
                alunos = await alunoRepository.ObterPorCodigosTurma(request.CodigosTurma);

            return Enumerable.DefaultIfEmpty(alunos.GroupBy(a => a.CodigoTurma));
        }
    }
}
