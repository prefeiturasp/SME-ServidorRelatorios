using MediatR;
using SME.SR.Workers.SGP.Commons;
using SME.SR.Workers.SGP.Infra;
using SME.SR.Workers.SGP.Models;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Workers.SGP.Queries
{
    public class ObterFrequenciaAlunoQueryHandler : IRequestHandler<ObterFrequenciaAlunoQuery, FrequenciaAluno>
    {
        private IFrequenciaAlunoRepository _frequenciaRepository;
        private IAulaRepository _aulaRepository;
        private IPeriodoEscolarRepository _periodoEscolarRepository;
        private ITipoCalendarioRepository _tipoCalendarioRepository;

        public ObterFrequenciaAlunoQueryHandler(IFrequenciaAlunoRepository frequenciaRepository,
                                                IAulaRepository aulaRepository,
                                                IPeriodoEscolarRepository periodoEscolarRepository,
                                                ITipoCalendarioRepository tipoCalendarioRepository)
        {
            this._frequenciaRepository = frequenciaRepository;
            this._aulaRepository = aulaRepository;
            this._periodoEscolarRepository = periodoEscolarRepository;
            this._tipoCalendarioRepository = tipoCalendarioRepository;
        }
        public async Task<FrequenciaAluno> Handle(ObterFrequenciaAlunoQuery request, CancellationToken cancellationToken)
        {
            var tipoCalendarioId = await _tipoCalendarioRepository.ObterPorAnoLetivoEModalidade(
                                                    request.Turma.AnoLetivo, 
                                                    request.Turma.ModalidadeTipoCalendario, 
                                                    request.Turma.Semestre);

            var frequenciaAluno = new FrequenciaAluno();
            if (request.PeriodoEscolar != null)
            {
                // Frequencia do bimestre
                frequenciaAluno = await _frequenciaRepository.ObterPorAlunoDataDisciplina(request.CodigoAluno,
                                                                                    request.PeriodoEscolar.PeriodoFim,
                                                                                    TipoFrequenciaAluno.PorDisciplina,
                                                                                    request.ComponenteCurricularCodigo);
                if (frequenciaAluno != null)
                    return frequenciaAluno;

                return new FrequenciaAluno()
                {
                    TotalAulas = await _aulaRepository.ObterAulasDadas(request.Turma.CodigoTurma,
                                                                    request.ComponenteCurricularCodigo,
                                                                    tipoCalendarioId,
                                                                    request.PeriodoEscolar.Bimestre)
                };
            }
            else
            {
                var periodosEscolaresTurma = await _periodoEscolarRepository.ObterPeriodosEscolaresPorTipoCalendario(tipoCalendarioId);
                foreach (var periodoEscolarTurma in periodosEscolaresTurma)
                {
                    var frequenciaAlunoPeriodo = await _frequenciaRepository.ObterPorAlunoBimestreAsync(request.CodigoAluno,
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
                        frequenciaAluno.TotalAulas += await _aulaRepository.ObterAulasDadas(request.Turma.CodigoTurma,
                                                                                            request.ComponenteCurricularCodigo,
                                                                                            tipoCalendarioId,
                                                                                            periodoEscolarTurma.Bimestre);
                }

                return frequenciaAluno;
            }
        }
    }
}
