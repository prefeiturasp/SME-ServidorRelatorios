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

        public ObterDadosRelatorioAlteracaoNotasCommandHandler(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));            
        }

        public  async Task<IEnumerable<TurmaAlteracaoNotasDto>> Handle(ObterDadosRelatorioAlteracaoNotasCommand request, CancellationToken cancellationToken)
        {            
            var listaTurmaAlteracaoNotasDto = new List<TurmaAlteracaoNotasDto>();

            var turmas = await ObterTurmas(request.FiltroRelatorio.Turma, request.FiltroRelatorio.CodigoUe, request.FiltroRelatorio.AnoLetivo);

            foreach (var turma in turmas)
            {
                var alunos = await mediator.Send(new ObterAlunosPorTurmaQuery()
                {
                    TurmaCodigo = turma.turma_id
                });

                

                // Mapear alunos para dto
            }

            

            return listaTurmaAlteracaoNotasDto;
        }

                
        private async Task<IEnumerable<Turma>> ObterTurmas(IEnumerable<long> FiltroTurmas, string codigoUe, long anoLetivo)
        {
            long[] turmasId = new long[] { };
            IEnumerable<Turma> turmas = new List<Turma>();

            if (FiltroTurmas.Any(c => c == -99))
            {
                turmas = await mediator.Send(new ObterTurmasPorUeEAnoLetivoQuery(codigoUe, anoLetivo));
            }
            else
            {
                turmasId = FiltroTurmas.ToArray();
                turmas = await mediator.Send(new ObterTurmasPorIdsQuery(turmasId));
            }

            return turmas;
        }

        private async Task<IEnumerable<HistoricoAlteracaoNotasDto>> ObterHistoricoAlteracaoNotasFechamento(string turmaCodigo)
                 => await mediator.Send(new ObterHistoricoNotasFechamentoPorTurmaIdQuery(long.Parse(turmaCodigo)));

        private async Task<IEnumerable<HistoricoAlteracaoNotasDto>> ObterHistoricoAlteracaoNotasConselhoClasse(string turmaCodigo)
                 => await mediator.Send(new ObterHistoricoNotasConselhoClassePorTurmaIdQuery(long.Parse(turmaCodigo)));
    }
}
