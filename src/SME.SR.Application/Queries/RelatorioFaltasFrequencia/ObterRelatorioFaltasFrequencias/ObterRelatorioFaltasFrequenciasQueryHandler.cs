using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterRelatorioFaltasFrequenciasQueryHandler : IRequestHandler<ObterRelatorioFaltasFrequenciasQuery, FiltroRelatorioFaltasFrequenciasDto>
    {
        private readonly IMediator mediator;

        public ObterRelatorioFaltasFrequenciasQueryHandler(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<FiltroRelatorioFaltasFrequenciasDto> Handle(ObterRelatorioFaltasFrequenciasQuery request, CancellationToken cancellationToken)
        {
            var turmas = await mediator.Send(new ObterTurmasPorAnoQuery(request.AnoLetivo, request.AnosEscolares));
            var alunos = await ObterAlunosPorAno(turmas);
            var faltasFrequencias = await ObterFaltasEFrequencias(turmas, request.Bimestres, request.ComponentesCurriculares);

            return await Task.FromResult(new FiltroRelatorioFaltasFrequenciasDto());
        }

        private async Task<IEnumerable<FrequenciaAluno>> ObterFaltasEFrequencias(IEnumerable<Turma> turmas, IEnumerable<int> bimestresFiltro, IEnumerable<long> componentesCurriculares)
        {
            var faltasFrequenciasAlunos = new List<FrequenciaAluno>();

            // Obter bimestres diferentes de "Final"
            var bimestres = bimestresFiltro.Where(c => c != 0);
            if (bimestres != null && bimestres.Any())
            {

                faltasFrequenciasAlunos.AddRange(await mediator.Send(new ObterFrequenciaAlunoPorComponentesBimestresETurmasQuery(turmas.Select(a => a.Codigo), bimestres, componentesCurriculares)));
            }

            // Verifica se foi solicitado bimestre final
            if (bimestresFiltro.Any(c => c == 0))
            {
                //faltasFrequenciasAlunos.AddRange(await mediator.Send(new ObterFrequenciaAlunoGlobalPorComponetnesBimestresETurmasQuery(turmas, componentesCurriculares)));
            }

            return faltasFrequenciasAlunos;
        }

        private async Task<IEnumerable<RelatorioFaltaFrequenciaAlunoDto>> ObterAlunosPorAno(IEnumerable<Turma> turmas)
        {
            var alunos = await mediator.Send(new ObterAlunosPorAnoQuery(turmas.Select(a => a.Codigo)));
            return alunos.Select(a => new RelatorioFaltaFrequenciaAlunoDto()
            {
                NomeAluno = a.NomeFinal,
                NomeTurma = turmas.First(t => t.Codigo == t.Codigo).Nome,
                NumeroChamada = a.NumeroChamada
            });
        }
    }
}
