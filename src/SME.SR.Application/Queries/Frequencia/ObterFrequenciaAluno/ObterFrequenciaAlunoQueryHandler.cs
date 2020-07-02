using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterFrequenciaAlunoQueryHandler : IRequestHandler<ObterFrequenciaAlunoQuery, FrequenciaAluno>
    {
        private readonly IFrequenciaAlunoRepository frequenciaRepository;
        private readonly IAulaRepository aulaRepository;
        private readonly IPeriodoEscolarRepository periodoEscolarRepository;
        private readonly ITipoCalendarioRepository tipoCalendarioRepository;

        public ObterFrequenciaAlunoQueryHandler(IFrequenciaAlunoRepository frequenciaRepository,
                                                IAulaRepository aulaRepository,
                                                IPeriodoEscolarRepository periodoEscolarRepository,
                                                ITipoCalendarioRepository tipoCalendarioRepository)
        {
            this.frequenciaRepository = frequenciaRepository ?? throw new ArgumentNullException(nameof(frequenciaRepository));
            this.aulaRepository = aulaRepository ?? throw new ArgumentNullException(nameof(aulaRepository));
            this.periodoEscolarRepository = periodoEscolarRepository ?? throw new ArgumentNullException(nameof(periodoEscolarRepository));
            this.tipoCalendarioRepository = tipoCalendarioRepository ?? throw new ArgumentNullException(nameof(tipoCalendarioRepository));
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
                                                                                    request.ComponenteCurricularCodigo);
                if (frequenciaAluno != null)
                    return frequenciaAluno;

                return new FrequenciaAluno()
                {
                    TotalAulas = await aulaRepository.ObterAulasDadas(request.Turma.Codigo,
                                                                    request.ComponenteCurricularCodigo,
                                                                    tipoCalendarioId,
                                                                    request.PeriodoEscolar.Bimestre)
                };
            }
            else
            {
                var periodosEscolaresTurma = await periodoEscolarRepository.ObterPeriodosEscolaresPorTipoCalendario(tipoCalendarioId);
                foreach (var periodoEscolarTurma in periodosEscolaresTurma)
                {
                    var frequenciaAlunoPeriodo = await frequenciaRepository.ObterPorAlunoBimestreAsync(request.CodigoAluno,
                                                                                    periodoEscolarTurma.Bimestre,
                                                                                    TipoFrequenciaAluno.PorDisciplina,
                                                                                    request.ComponenteCurricularCodigo);
                    if (frequenciaAlunoPeriodo != null)
                    {
                        frequenciaAluno.TotalAulas += frequenciaAlunoPeriodo.TotalAulas;
                        frequenciaAluno.TotalAusencias += frequenciaAlunoPeriodo.TotalAusencias;
                        frequenciaAluno.TotalCompensacoes += frequenciaAlunoPeriodo.TotalCompensacoes;
                    }
                    else
                        // Se não tem ausencia não vai ter registro de frequencia então soma apenas aulas do bimestre
                        frequenciaAluno.TotalAulas += await aulaRepository.ObterAulasDadas(request.Turma.Codigo,
                                                                                            request.ComponenteCurricularCodigo,
                                                                                            tipoCalendarioId,
                                                                                            periodoEscolarTurma.Bimestre);
                }

                return frequenciaAluno;
            }
        }
    }
}
