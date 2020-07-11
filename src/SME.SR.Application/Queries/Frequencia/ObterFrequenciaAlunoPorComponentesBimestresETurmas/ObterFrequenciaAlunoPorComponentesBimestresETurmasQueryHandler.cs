using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterFrequenciaAlunoPorComponentesBimestresETurmasQueryHandler : IRequestHandler<ObterFrequenciaAlunoPorComponentesBimestresETurmasQuery, IEnumerable<FrequenciaAluno>>
    {
        private readonly IFrequenciaAlunoRepository frequenciaAlunoRepository;

        public ObterFrequenciaAlunoPorComponentesBimestresETurmasQueryHandler(IFrequenciaAlunoRepository frequenciaAlunoRepository)
        {
            this.frequenciaAlunoRepository = frequenciaAlunoRepository ?? throw new ArgumentNullException(nameof(frequenciaAlunoRepository));
        }

        public async Task<IEnumerable<FrequenciaAluno>> Handle(ObterFrequenciaAlunoPorComponentesBimestresETurmasQuery request, CancellationToken cancellationToken)
            => await frequenciaAlunoRepository.ObterFrequenciaPorComponentesBimestresTurmas(request.ComponentesCurriculares.Select(c => c.ToString()).ToArray()
                                                                                          , request.Bimestres.ToArray()
                                                                                          , request.TurmasCodigos.ToArray());
    }
}
