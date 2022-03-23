using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterFrequenciasAlunosConsolidadoQueryHandler : IRequestHandler<ObterFrequenciasAlunosConsolidadoQuery, IEnumerable<FrequenciaAlunoConsolidadoRelatorioDto>>
    {
        private readonly IFrequenciaAlunoRepository frequenciaRepository;

        public ObterFrequenciasAlunosConsolidadoQueryHandler(IFrequenciaAlunoRepository frequenciaRepository)
        {
            this.frequenciaRepository = frequenciaRepository ?? throw new ArgumentNullException(nameof(frequenciaRepository));
        }

        public async Task<IEnumerable<FrequenciaAlunoConsolidadoRelatorioDto>> Handle(ObterFrequenciasAlunosConsolidadoQuery request, CancellationToken cancellationToken)
            => await frequenciaRepository.ObterFrequenciaAlunosRelatorio(request.Codigosturma, request.Bimestre.ToString(), request.ComponenteCurricularId);        
    }
}
