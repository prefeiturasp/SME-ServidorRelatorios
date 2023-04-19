using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra.Dtos;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterFrequenciaAlunoPorCodigoBimestreQueryHandler : IRequestHandler<ObterFrequenciaAlunoPorCodigoBimestreQuery, IEnumerable<FrequenciaAlunoConsolidadoDto>>
    {
        private readonly IFrequenciaAlunoRepository frequenciaAlunoRepository;
        public ObterFrequenciaAlunoPorCodigoBimestreQueryHandler(IFrequenciaAlunoRepository frequenciaAlunoRepository)
        {
            this.frequenciaAlunoRepository = frequenciaAlunoRepository ?? throw new ArgumentNullException(nameof(frequenciaAlunoRepository));
        }
        public Task<IEnumerable<FrequenciaAlunoConsolidadoDto>> Handle(ObterFrequenciaAlunoPorCodigoBimestreQuery request, CancellationToken cancellationToken)
        {
            return frequenciaAlunoRepository.ObterFrequenciaAlunosPorCodigoBimestre(request.CodigosAlunos, request.Bimestre,request.TurmaCodigo,request.TipoFrequencia,request.ComponentesCurricularesId);
        }
    }
}
