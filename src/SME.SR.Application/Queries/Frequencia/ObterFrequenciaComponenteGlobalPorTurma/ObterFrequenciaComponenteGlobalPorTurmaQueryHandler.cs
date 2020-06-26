using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterFrequenciaComponenteGlobalPorTurmaQueryHandler : IRequestHandler<ObterFrequenciaComponenteGlobalPorTurmaQuery , IEnumerable<FrequenciaAluno>>
    {
        private IFrequenciaAlunoRepository frequenciaRepository;
        private ITipoCalendarioRepository tipoCalendarioRepository;

        public ObterFrequenciaComponenteGlobalPorTurmaQueryHandler(IFrequenciaAlunoRepository frequenciaRepository,
                                                                   ITipoCalendarioRepository tipoCalendarioRepository)
        {
            this.frequenciaRepository = frequenciaRepository ?? throw new ArgumentNullException(nameof(frequenciaRepository));
            this.tipoCalendarioRepository = tipoCalendarioRepository ?? throw new ArgumentNullException(nameof(tipoCalendarioRepository));
        }

        public async Task<IEnumerable<FrequenciaAluno>> Handle(ObterFrequenciaComponenteGlobalPorTurmaQuery request, CancellationToken cancellationToken)
        {
            var tipoCalendarioId = await tipoCalendarioRepository.ObterPorAnoLetivoEModalidade(request.AnoLetivo, request.Modalidade, request.Semestre);

            return await frequenciaRepository.ObterFrequenciaDisciplinaGlobalPorTurma(request.TurmaCodigo, tipoCalendarioId);
        }
    }
}
