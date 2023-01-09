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
    public class ObterFrequenciaComponenteGlobalPorTurmaQueryHandler : IRequestHandler<ObterFrequenciaComponenteGlobalPorTurmaQuery, IEnumerable<FrequenciaAluno>>
    {
        private readonly IFrequenciaAlunoRepository frequenciaAlunoRepository;

        private readonly IMediator mediator;

        public ObterFrequenciaComponenteGlobalPorTurmaQueryHandler(IMediator mediator, IFrequenciaAlunoRepository frequenciaAlunoRepository, IRegistroFrequenciaRepository registroFrequenciaRepository)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.frequenciaAlunoRepository = frequenciaAlunoRepository ?? throw new ArgumentNullException(nameof(frequenciaAlunoRepository));
        }

        public async Task<IEnumerable<FrequenciaAluno>> Handle(ObterFrequenciaComponenteGlobalPorTurmaQuery request, CancellationToken cancellationToken)
        {
            var componentesCurricularesId = request.ComponentesCurricularesPorTurma
                .Select(cc => cc.ComponenteCurricularId.ToString())
                .Distinct().ToArray();

            var frequenciaTurma = await frequenciaAlunoRepository
                .ObterFrequenciaDisciplinaGlobalPorTurma(request.TurmasCodigo, componentesCurricularesId, request.TipoCalendarioId);

            var aulas = await mediator
                .Send(new ObterAulasPorTurmasComponentesTipoCalendarioBimestresQuery(request.TurmasCodigo, request.TipoCalendarioId, componentesCurricularesId, new int[] { }));

            return await TratarFrequenciaAnualAluno(frequenciaTurma, request.Bimestres, request.ComponentesCurricularesPorTurma, aulas, request.AlunosDatasMatricula);
        }

        private async Task<IEnumerable<FrequenciaAluno>> TratarFrequenciaAnualAluno(IEnumerable<FrequenciaAluno> frequenciaTurma, int[] bimestres, IEnumerable<(string CodigoTurma, long ComponenteCurricularId)> componentesCurricularesPorTurma, IEnumerable<TurmaComponenteDataAulaQuantidadeDto> aulas, IEnumerable<(string codigoAluno, DateTime dataMatricula, DateTime? dataSituacao)> alunosDatasMatriculas)
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

                var dadosMatricula = alunosDatasMatriculas.SingleOrDefault(a => a.codigoAluno.Equals(alunoComponente.Key.CodigoAluno));
                var turmaCodigo = componentesCurricularesPorTurma.FirstOrDefault(cc => cc.ComponenteCurricularId.ToString() == alunoComponente.Key.DisciplinaId).CodigoTurma;
                var turma = await mediator.Send(new ObterTurmaQuery(turmaCodigo));

                foreach (var bimestre in bimestres)
                {
                    var frequenciaBimestre = alunoComponente.FirstOrDefault(c => c.Bimestre == bimestre);
                    var disciplinaId = !string.IsNullOrEmpty(frequenciaAluno.DisciplinaId) ? Convert.ToInt64(frequenciaAluno.DisciplinaId) : 0;

                    frequenciaAluno.TotalAulas += frequenciaBimestre?.TotalAulas ?? 0;
                    frequenciaAluno.TotalAusencias += frequenciaBimestre?.TotalAusencias ?? 0;
                    frequenciaAluno.TotalCompensacoes += frequenciaBimestre?.TotalCompensacoes ?? 0;                    

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
