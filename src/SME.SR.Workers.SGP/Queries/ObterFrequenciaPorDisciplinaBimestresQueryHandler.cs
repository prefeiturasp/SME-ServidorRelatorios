using MediatR;
using SME.SR.Workers.SGP.Commons;
using SME.SR.Workers.SGP.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Workers.SGP.Queries
{
    public class ObterFrequenciaPorDisciplinaBimestresQueryHandler : IRequestHandler<ObterFrequenciaPorDisciplinaBimestresQuery, IEnumerable<FrequenciaAluno>>
    {
        private IFrequenciaAlunoRepository _frequenciaRepository;

        public ObterFrequenciaPorDisciplinaBimestresQueryHandler(IFrequenciaAlunoRepository frequenciaRepository)
        {
            this._frequenciaRepository = frequenciaRepository;
        }

        public async Task<IEnumerable<FrequenciaAluno>> Handle(ObterFrequenciaPorDisciplinaBimestresQuery request, CancellationToken cancellationToken)
        {
            return await _frequenciaRepository.ObterFrequenciaPorDisciplinaBimestres(request.CodigoAluno, request.CodigoTurma, request.Bimestre);
        }
    }
}
