using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterAlunosPorTurmasRelatorioFrequenciaQueryHandler : IRequestHandler<ObterAlunosPorTurmasRelatorioFrequenciaQuery, IEnumerable<AlunoTurma>>
    {
        private readonly IAlunoRepository alunoRepository;

        public ObterAlunosPorTurmasRelatorioFrequenciaQueryHandler(IAlunoRepository alunoRepository)
        {
            this.alunoRepository = alunoRepository ?? throw new ArgumentNullException(nameof(alunoRepository));
        }

        public async Task<IEnumerable<AlunoTurma>> Handle(ObterAlunosPorTurmasRelatorioFrequenciaQuery request, CancellationToken cancellationToken)
        {
            var alunosRetorno = new List<Aluno>();
            var alunos = await alunoRepository.ObterPorCodigosTurma(request.TurmasCodigos.ToArray());

            if (alunos == null || !alunos.Any())
                throw new NegocioException("Alunos não encontrados");

            foreach (var alunosTurma in alunos.GroupBy(a => a.CodigoTurma))
                alunosRetorno.AddRange(alunosTurma.GroupBy(a => a.CodigoAluno).SelectMany(x => x.OrderByDescending(y => y.DataSituacao).Take(1)));

            if (alunosRetorno == null || !alunosRetorno.Any())
                throw new NegocioException("Alunos não encontrados");

            return alunosRetorno.Select(a => new AlunoTurma()
            {
                Nome = a.NomeAluno,
                NumeroChamada = a.NumeroAlunoChamada,
                CodigoAluno = a.CodigoAluno,
                TurmaCodigo = a.CodigoTurma.ToString(),
                NomeFinal = a.ObterNomeFinal(),
                SituacaoMatricula = a.CodigoSituacaoMatricula,
                DataSituacao = a.DataSituacao
            });
        }
    }
}
