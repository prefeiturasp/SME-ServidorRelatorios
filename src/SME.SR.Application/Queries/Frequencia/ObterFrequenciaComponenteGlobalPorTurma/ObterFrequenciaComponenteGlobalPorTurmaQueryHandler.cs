﻿using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterFrequenciaComponenteGlobalPorTurmaQueryHandler : IRequestHandler<ObterFrequenciaComponenteGlobalPorTurmaQuery, IEnumerable<FrequenciaAluno>>
    {
        private readonly IFrequenciaAlunoRepository frequenciaRepository;
        private readonly IMediator mediator;

        public ObterFrequenciaComponenteGlobalPorTurmaQueryHandler(IMediator mediator, IFrequenciaAlunoRepository frequenciaRepository)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.frequenciaRepository = frequenciaRepository ?? throw new ArgumentNullException(nameof(frequenciaRepository));
        }

        public async Task<IEnumerable<FrequenciaAluno>> Handle(ObterFrequenciaComponenteGlobalPorTurmaQuery request, CancellationToken cancellationToken)
        {
            var componentesCurricularesId = request.ComponentesCurricularesPorTurma.Select(cc => cc.ComponenteCurricularId.ToString()).Distinct().ToArray();

            var frequenciaTurma = await frequenciaRepository.ObterFrequenciaDisciplinaGlobalPorTurma(request.TurmasCodigo, componentesCurricularesId, request.TipoCalendarioId);

            return await TratarFrequenciaAnualAluno(frequenciaTurma, request.Bimestres, request.ComponentesCurricularesPorTurma, request.TipoCalendarioId);
        }

        private async Task<IEnumerable<FrequenciaAluno>> TratarFrequenciaAnualAluno(IEnumerable<FrequenciaAluno> frequenciaTurma, int[] bimestres, IEnumerable<(string CodigoTurma, long ComponenteCurricularId)> componentesCurricularesPorTurma, long tipoCalendarioId)
        {
            var frequenciaGlobalAlunos = new List<FrequenciaAluno>();

            var agrupamentoAlunoComponente = frequenciaTurma.GroupBy(g => (g.CodigoAluno, g.DisciplinaId));
            foreach (var alunoComponente in agrupamentoAlunoComponente)
            {
                var frequenciaAluno = new FrequenciaAluno()
                {
                    CodigoAluno = alunoComponente.Key.CodigoAluno,
                    DisciplinaId = alunoComponente.Key.DisciplinaId
                };

                var turmaCodigo = componentesCurricularesPorTurma.FirstOrDefault(cc => cc.ComponenteCurricularId.ToString() == alunoComponente.Key.DisciplinaId).CodigoTurma;

                foreach (var bimestre in bimestres)
                {
                    var frequenciaBimestre = alunoComponente.FirstOrDefault(c => c.Bimestre == bimestre);
                    var disciplinaId = !string.IsNullOrEmpty(frequenciaAluno.DisciplinaId) ? Convert.ToInt64(frequenciaAluno.DisciplinaId) : 0;

                    frequenciaAluno.TotalAulas += frequenciaBimestre?.TotalAulas ??
                                        await mediator.Send(new ObterAulasDadasNoBimestreQuery(turmaCodigo, tipoCalendarioId, disciplinaId, bimestre));

                    frequenciaAluno.TotalAusencias += frequenciaBimestre?.TotalAusencias ?? 0;
                    frequenciaAluno.TotalCompensacoes += frequenciaBimestre?.TotalCompensacoes ?? 0;

                    var turma = await mediator.Send(new ObterTurmaQuery(turmaCodigo));

                    // Particularidade de cálculo de frequência para 2020.
                    if (turma.AnoLetivo.Equals(2020))
                        frequenciaAluno.AdicionarFrequenciaBimestre(bimestre, frequenciaBimestre?.PercentualFrequencia ?? 100);
                }

                frequenciaGlobalAlunos.Add(frequenciaAluno);
            }

            return frequenciaGlobalAlunos;
        }
    }
}
