using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterAlunosPorAnoQueryHandler : IRequestHandler<ObterAlunosPorAnoQuery, IEnumerable<AlunoTurma>>
    {
        private readonly IAlunoRepository alunoRepository;

        public ObterAlunosPorAnoQueryHandler(IAlunoRepository alunoRepository)
        {
            this.alunoRepository = alunoRepository ?? throw new ArgumentNullException(nameof(alunoRepository));
        }

        public async Task<IEnumerable<AlunoTurma>> Handle(ObterAlunosPorAnoQuery request, CancellationToken cancellationToken)
        {
            var alunos = await alunoRepository.ObterPorCodigosTurma(request.TurmasCodigos.ToArray());

            return alunos.Select(a => new AlunoTurma()
            {
                Nome = a.ObterNomeFinal(),
                NumeroChamada = a.NumeroAlunoChamada,
                CodigoAluno = a.CodigoAluno,
                TurmaCodigo = a.CodigoTurma.ToString(),
                NomeFinal = a.ObterNomeFinal(),
                SituacaoMatricula= a.CodigoSituacaoMatricula
            });
        }
    }
}
