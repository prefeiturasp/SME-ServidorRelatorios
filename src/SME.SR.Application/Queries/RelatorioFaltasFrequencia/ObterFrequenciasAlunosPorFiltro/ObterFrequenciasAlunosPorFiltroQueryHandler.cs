using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterFrequenciasAlunosPorFiltroQueryHandler : IRequestHandler<ObterFrequenciasAlunosPorFiltroQuery, IEnumerable<FrequenciaAlunoRetornoDto>>
    {
        private readonly IFrequenciaAlunoRepository frequenciaRepository;

        public ObterFrequenciasAlunosPorFiltroQueryHandler(IFrequenciaAlunoRepository frequenciaRepository)
        {
            this.frequenciaRepository = frequenciaRepository ?? throw new ArgumentNullException(nameof(frequenciaRepository));
        }

        public async Task<IEnumerable<FrequenciaAlunoRetornoDto>> Handle(ObterFrequenciasAlunosPorFiltroQuery request, CancellationToken cancellationToken)
            => await frequenciaRepository.ObterFrequenciasAlunosPorFiltro(request.Codigosturma, request.ComponenteCurricularId, request.Bimestre);
    }
}
