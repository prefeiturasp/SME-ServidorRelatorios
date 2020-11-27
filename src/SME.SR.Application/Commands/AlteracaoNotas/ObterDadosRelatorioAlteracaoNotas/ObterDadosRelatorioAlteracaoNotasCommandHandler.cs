using MediatR;
using SME.SR.Data;
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
    public class ObterDadosRelatorioAlteracaoNotasCommandHandler : IRequestHandler<ObterDadosRelatorioAlteracaoNotasCommand, IEnumerable<TurmaAlteracaoNotasDto>>
    {
        private readonly IMediator mediator;
        private readonly ITurmaRepository TurmaRepository;

        public ObterDadosRelatorioAlteracaoNotasCommandHandler(IMediator mediator, ITurmaRepository TurmaRepository)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.TurmaRepository = TurmaRepository ?? throw new ArgumentNullException(nameof(TurmaRepository));
        }

        public  async Task<IEnumerable<TurmaAlteracaoNotasDto>> Handle(ObterDadosRelatorioAlteracaoNotasCommand request, CancellationToken cancellationToken)
        {
            var listaTurmaAlteracaoNotasDto = new List<TurmaAlteracaoNotasDto>();

            long[] turmasId = new long[] { };
            IEnumerable<Turma> turmas = new List<Turma>();

            if (request.FiltroRelatorio.Turmas.Contains(-99))
            {
                turmas = await mediator.Send(new ObterTurmasPorUeEAnoLetivoQuery(request.FiltroRelatorio.CodigoUe, request.FiltroRelatorio.AnoLetivo));
            }
            else
            {
                turmasId = request.FiltroRelatorio.Turmas.ToArray();
                turmas = await mediator.Send(new ObterTurmasPorIdsQuery(turmasId));
            }

            foreach(var turma in turmas)
            {
                var alunos = await mediator.Send(new ObterAlunosPorTurmaQuery()
                {
                    TurmaCodigo = turma.Codigo
                });
            }


            return listaTurmaAlteracaoNotasDto;
        }      
    }
}
