using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterFrequenciaGeralAlunosPorTurmaEBimestreQueryHandler : IRequestHandler<ObterFrequenciaGeralAlunosPorTurmaEBimestreQuery, IEnumerable<FrequenciaAluno>>
    {
        private readonly IFrequenciaAlunoRepository frequenciaAlunoRepository;

        public ObterFrequenciaGeralAlunosPorTurmaEBimestreQueryHandler(IFrequenciaAlunoRepository frequenciaAlunoRepository)
        {
            this.frequenciaAlunoRepository = frequenciaAlunoRepository ?? throw new ArgumentNullException(nameof(frequenciaAlunoRepository));
        }

        public async Task<IEnumerable<FrequenciaAluno>> Handle(ObterFrequenciaGeralAlunosPorTurmaEBimestreQuery request, CancellationToken cancellationToken)
        {
            var frequenciaAluno = await frequenciaAlunoRepository.ObterFrequenciaGeralAlunosPorTurmaEBimestre(request.TurmaId, request.AlunoCodigo, request.Bimestres);

            return frequenciaAluno;
        }
    }
}
