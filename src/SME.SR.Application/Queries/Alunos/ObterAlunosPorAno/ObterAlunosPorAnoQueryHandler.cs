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
        private readonly IMediator mediator;
        private readonly IAlunoRepository alunoRepository;

        public ObterAlunosPorAnoQueryHandler(IMediator mediator, IAlunoRepository alunoRepository)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.alunoRepository = alunoRepository ?? throw new ArgumentNullException(nameof(alunoRepository));
        }

        public async Task<IEnumerable<AlunoTurma>> Handle(ObterAlunosPorAnoQuery request, CancellationToken cancellationToken)
        {
            var turmas = await mediator.Send(new ObterTurmasPorAnoQuery(request.AnoLetivo, request.AnosEscolares));
            var alunos = await alunoRepository.ObterPorCodigosTurma(turmas.Select(a => a.Codigo).ToArray());

            return alunos.Select(a => new AlunoTurma()
            {
                Nome = a.NomeAluno,
                NumeroChamada = a.NumeroAlunoChamada,
                Turma = turmas.First(t => t.Codigo == a.CodigoTurma.ToString()).Nome
            });
        }
    }
}
