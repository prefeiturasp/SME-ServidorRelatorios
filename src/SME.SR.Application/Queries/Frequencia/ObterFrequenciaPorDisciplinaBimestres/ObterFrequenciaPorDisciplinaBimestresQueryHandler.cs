using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
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
            return await _frequenciaRepository.ObterFrequenciaPorDisciplinaBimestres(request.CodigoTurma, request.CodigoAluno, request.Bimestre);
        }
    }
}
