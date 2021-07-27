using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterFrequenciasAlunosPorTurmasQueryHandler : IRequestHandler<ObterFrequenciasAlunosPorTurmasQuery, IEnumerable<FrequenciaAlunoRetornoDto>>
    {
        private readonly IFrequenciaAlunoRepository frequenciaRepository;

        public ObterFrequenciasAlunosPorTurmasQueryHandler(IFrequenciaAlunoRepository frequenciaRepository)
        {
            this.frequenciaRepository = frequenciaRepository ?? throw new ArgumentNullException(nameof(frequenciaRepository));
        }

        public async Task<IEnumerable<FrequenciaAlunoRetornoDto>> Handle(ObterFrequenciasAlunosPorTurmasQuery request, CancellationToken cancellationToken)
            => await frequenciaRepository.ObterFrequenciasAlunosPorTurmas(request.Codigosturma);
    }
}
