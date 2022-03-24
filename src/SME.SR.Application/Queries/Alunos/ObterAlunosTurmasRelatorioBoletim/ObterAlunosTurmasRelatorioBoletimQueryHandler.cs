using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterAlunosTurmasRelatorioBoletimQueryHandler : IRequestHandler<ObterAlunosTurmasRelatorioBoletimQuery, IEnumerable<IGrouping<string, Aluno>>>
    {
        private IAlunoRepository alunoRepository;

        public ObterAlunosTurmasRelatorioBoletimQueryHandler(IAlunoRepository alunoRepository)
        {
            this.alunoRepository = alunoRepository;
        }

        public async Task<IEnumerable<IGrouping<string, Aluno>>> Handle(ObterAlunosTurmasRelatorioBoletimQuery request, CancellationToken cancellationToken)
        {
            var alunos = Enumerable.Empty<Aluno>();

            if (request.CodigosAlunos != null && request.CodigosAlunos.Length > 0)
                alunos = await alunoRepository.ObterPorCodigosAlunoETurma(request.CodigosTurma, request.CodigosAlunos);
            else
                alunos = await alunoRepository.ObterPorCodigosTurma(request.CodigosTurma);

            if (!alunos.Any())
                throw new NegocioException("Não foi possível localizar os alunos");
            else
            {
                return request.TrazerAlunosInativos ? alunos.OrderBy(a => a.ObterNomeFinal()).GroupBy(a => a.CodigoAluno.ToString()) 
                                                    : alunos.Where(al => al.CodigoSituacaoMatricula == SituacaoMatriculaAluno.Ativo 
                                                        || al.CodigoSituacaoMatricula == SituacaoMatriculaAluno.Concluido)
                                                        .OrderBy(a => a.ObterNomeFinal()).GroupBy(a => a.CodigoAluno.ToString());
            }
                
        }
    }
}
