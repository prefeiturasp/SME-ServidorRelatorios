using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterAlunosTurmasRelatorioBoletimQueryHandler : IRequestHandler<ObterAlunosTurmasRelatorioBoletimQuery, IEnumerable<IGrouping<string, Aluno>>>
    {
        private readonly IAlunoRepository alunoRepository;

        public ObterAlunosTurmasRelatorioBoletimQueryHandler(IAlunoRepository alunoRepository)
        {
            this.alunoRepository = alunoRepository;
        }

        public async Task<IEnumerable<IGrouping<string, Aluno>>> Handle(ObterAlunosTurmasRelatorioBoletimQuery request, CancellationToken cancellationToken)
        {
            var alunos = Enumerable.Empty<Aluno>();
            var alunosOrdenadosPorSituacao = Enumerable.Empty<Aluno>();

            if (request.CodigosAlunos?.Length > 0)
            {
                alunos = await alunoRepository.ObterPorCodigosAlunoETurma(request.CodigosTurma, request.CodigosAlunos);
                alunosOrdenadosPorSituacao = ObterAlunosPorUltimaSituacao(alunos, request.CodigosTurma);
            }
            else
            {
                alunos = await alunoRepository.ObterPorCodigosTurma(request.CodigosTurma);
                alunosOrdenadosPorSituacao = ObterAlunosPorUltimaSituacao(alunos, request.CodigosTurma);
            }

            if (!alunosOrdenadosPorSituacao.Any())
                throw new NegocioException("Não foi possível localizar os alunos");

            var resultadoAlunos = request.TrazerAlunosInativos ? alunosOrdenadosPorSituacao.OrderBy(a => a.ObterNomeFinal()).GroupBy(a => a.CodigoAluno.ToString())
                                                : alunosOrdenadosPorSituacao.Where(al => al.CodigoSituacaoMatricula == SituacaoMatriculaAluno.Ativo
                                                    || al.CodigoSituacaoMatricula == SituacaoMatriculaAluno.Concluido)
                                                    .OrderBy(a => a.ObterNomeFinal()).GroupBy(a => a.CodigoAluno.ToString());
            if (!resultadoAlunos.Any())
                throw new NegocioException("Não foi possível localizar alunos com os filtros definidos.");

            return resultadoAlunos;
        }

        private IEnumerable<Aluno> ObterAlunosPorUltimaSituacao(IEnumerable<Aluno> listaAlunos, params string[] codigosTurmas)
        {
            var listaTemporaria = new List<Aluno>();
            var listaAlunosOrdenada = new List<Aluno>();

            foreach (var item in listaAlunos)
            {
                listaTemporaria = listaAlunos.Where(x => x.CodigoAluno == item.CodigoAluno && codigosTurmas.ToList().Contains(x.CodigoTurma.ToString())).ToList();
                listaAlunosOrdenada.AddRange(listaTemporaria.OrderByDescending(x => x.DataSituacao).Take(1));
                listaTemporaria.Clear();
            }

            return listaAlunosOrdenada.DistinctBy(x => x.CodigoAluno);
        }
    }
}