using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterFrequenciaPorDisciplinaBimestresQueryHandler : IRequestHandler<ObterFrequenciaPorDisciplinaBimestresQuery, IEnumerable<FrequenciaAluno>>
    {
        private readonly IFrequenciaAlunoRepository frequenciaRepository;

        public ObterFrequenciaPorDisciplinaBimestresQueryHandler(IFrequenciaAlunoRepository frequenciaRepository)
        {
            this.frequenciaRepository = frequenciaRepository ?? throw new ArgumentNullException(nameof(frequenciaRepository));
        }

        public async Task<IEnumerable<FrequenciaAluno>> Handle(ObterFrequenciaPorDisciplinaBimestresQuery request, CancellationToken cancellationToken)
        {
            return await frequenciaRepository.ObterFrequenciaPorDisciplinaBimestres(request.CodigoTurma, request.CodigoAluno, request.Bimestre);
        }
    }
}
