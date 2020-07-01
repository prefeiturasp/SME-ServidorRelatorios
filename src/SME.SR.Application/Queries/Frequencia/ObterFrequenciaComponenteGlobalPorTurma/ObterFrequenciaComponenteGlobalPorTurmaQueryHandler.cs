using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterFrequenciaComponenteGlobalPorTurmaQueryHandler : IRequestHandler<ObterFrequenciaComponenteGlobalPorTurmaQuery , IEnumerable<FrequenciaAluno>>
    {
        private IFrequenciaAlunoRepository frequenciaRepository;

        public ObterFrequenciaComponenteGlobalPorTurmaQueryHandler(IFrequenciaAlunoRepository frequenciaRepository)
        {
            this.frequenciaRepository = frequenciaRepository ?? throw new ArgumentNullException(nameof(frequenciaRepository));
        }

        public async Task<IEnumerable<FrequenciaAluno>> Handle(ObterFrequenciaComponenteGlobalPorTurmaQuery request, CancellationToken cancellationToken)
        {
            return await frequenciaRepository.ObterFrequenciaDisciplinaGlobalPorTurma(request.TurmaCodigo, request.TipoCalendarioId);
        }
    }
}
