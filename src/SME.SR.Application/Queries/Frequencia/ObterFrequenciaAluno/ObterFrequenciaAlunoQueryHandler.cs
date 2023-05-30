using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterFrequenciaAlunoQueryHandler : IRequestHandler<ObterFrequenciaAlunoQuery, FrequenciaAluno>
    {
        private readonly IFrequenciaAlunoRepository frequenciaRepository;
        private readonly IPeriodoEscolarRepository periodoEscolarRepository;
        private readonly ITipoCalendarioRepository tipoCalendarioRepository;
        private readonly IRegistroFrequenciaRepository registroFrequenciaRepository;

        public ObterFrequenciaAlunoQueryHandler(IFrequenciaAlunoRepository frequenciaRepository,
                                                IPeriodoEscolarRepository periodoEscolarRepository,
                                                ITipoCalendarioRepository tipoCalendarioRepository,
                                                IRegistroFrequenciaRepository registroFrequenciaRepository)
        {
            this.frequenciaRepository = frequenciaRepository ?? throw new ArgumentNullException(nameof(frequenciaRepository));
            this.periodoEscolarRepository = periodoEscolarRepository ?? throw new ArgumentNullException(nameof(periodoEscolarRepository));
            this.tipoCalendarioRepository = tipoCalendarioRepository ?? throw new ArgumentNullException(nameof(tipoCalendarioRepository));
            this.registroFrequenciaRepository = registroFrequenciaRepository ?? throw new ArgumentNullException(nameof(registroFrequenciaRepository));

        }
        public async Task<FrequenciaAluno> Handle(ObterFrequenciaAlunoQuery request, CancellationToken cancellationToken)
        {
            var tipoCalendarioId = await tipoCalendarioRepository.ObterPorAnoLetivoEModalidade(
                                                    request.Turma.AnoLetivo,
                                                    request.Turma.ModalidadeTipoCalendario,
                                                    request.Turma.Semestre);

            var frequenciaAluno = new FrequenciaAluno();
            if (request.PeriodoEscolar != null)
            {
                // Frequencia do bimestre
                frequenciaAluno = await frequenciaRepository.ObterPorAlunoDataDisciplina(request.CodigoAluno,
                                                                                    request.PeriodoEscolar.PeriodoFim,
                                                                                    TipoFrequenciaAluno.PorDisciplina,
                                                                                    request.ComponenteCurricularCodigo,
                                                                                    request.Turma.Codigo);
                if (frequenciaAluno != null)
                    return frequenciaAluno;

                var totalAulas = await registroFrequenciaRepository.ObterTotalAulasPorDisciplinaETurmaEBimestre(
                        new string[] { request.Turma.Codigo },
                        new string[] { request.ComponenteCurricularCodigo },
                        tipoCalendarioId,
                        new int[] { request.PeriodoEscolar.Bimestre });

                return new FrequenciaAluno()
                {
                    TotalAulas = totalAulas.FirstOrDefault() != null ? totalAulas.FirstOrDefault().AulasQuantidade : 0
                };
            }
            else
            {
                var periodosEscolaresTurma = await periodoEscolarRepository.ObterPeriodosEscolaresPorTipoCalendario(tipoCalendarioId);

                var totalAulas = await registroFrequenciaRepository.ObterTotalAulasPorDisciplinaETurmaEBimestre(
                       new string[] { request.Turma.Codigo },
                       new string[] { request.ComponenteCurricularCodigo },
                       tipoCalendarioId,
                       periodosEscolaresTurma.Select(a => a.Bimestre).ToArray());

                foreach (var periodoEscolarTurma in periodosEscolaresTurma)
                {
                    var frequenciaAlunoPeriodo = await frequenciaRepository.ObterPorAlunoBimestreAsync(request.CodigoAluno,
                                                                                    periodoEscolarTurma.Bimestre,
                                                                                    TipoFrequenciaAluno.PorDisciplina,
                                                                                    request.ComponenteCurricularCodigo,
                                                                                    request.Turma.Codigo);

                    frequenciaAluno.AdicionarFrequenciaBimestre(periodoEscolarTurma.Bimestre, frequenciaAlunoPeriodo != null ? frequenciaAlunoPeriodo.PercentualFrequencia : 100);

                    if (frequenciaAlunoPeriodo != null)
                    {
                        frequenciaAluno.Id = frequenciaAlunoPeriodo.Id;
                        frequenciaAluno.TotalAulas += frequenciaAlunoPeriodo.TotalAulas;
                        frequenciaAluno.TotalAusencias += frequenciaAlunoPeriodo.TotalAusencias;
                        frequenciaAluno.TotalCompensacoes += frequenciaAlunoPeriodo.TotalCompensacoes;
                    }
                    else
                    {
                        // Se não tem ausencia não vai ter registro de frequencia então soma apenas aulas do bimestre
                        var aula = totalAulas.FirstOrDefault(a => a.Bimestre == periodoEscolarTurma.Bimestre);
                        frequenciaAluno.TotalAulas += aula != null ? aula.AulasQuantidade : 0;

                    }

                }

                return frequenciaAluno;
            }
        }
    }
}