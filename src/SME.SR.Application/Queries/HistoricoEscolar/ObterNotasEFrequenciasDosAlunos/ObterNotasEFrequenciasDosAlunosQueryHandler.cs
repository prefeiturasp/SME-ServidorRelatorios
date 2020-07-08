using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterNotasEFrequenciasDosAlunosQueryHandler : IRequestHandler<ObterNotasEFrequenciasDosAlunosQuery, IEnumerable<HistoricoEscolarDTO>>
    {
        private readonly IMediator mediator;

        public ObterNotasEFrequenciasDosAlunosQueryHandler(IMediator mediator)
        {
            this.mediator = mediator ?? throw new System.ArgumentNullException(nameof(mediator));
        }

        public async Task<IEnumerable<HistoricoEscolarDTO>> Handle(ObterNotasEFrequenciasDosAlunosQuery request, CancellationToken cancellationToken)
        {
            
            if (request.CodigoAlunos.Any())
            {
                var pareceresConclusivosCodigos = await mediator.Send(new ObterPareceresConclusivosQuery(request.CodigoAlunos));
                if (!pareceresConclusivosCodigos.Any())
                    throw new NegocioException("Não foi possível localizar os pareceres conclusivos.");

                //Obter as turmas dos Alunos
                var turmasDosAlunos = await mediator.Send(new ObterTurmasPorAlunosQuery(request.CodigoAlunos, pareceresConclusivosCodigos));
                if (turmasDosAlunos.Any())
                {
                    var turmasAgrupadas = turmasDosAlunos.GroupBy(a => a.AlunoCodigo).ToList();
                }


            } else
            {
                 //Obter os alunos da turma

            }

        }
    }
}
