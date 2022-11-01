using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterAulasPorTurmasComponentesTipoCalendarioBimestresQueryHandler : IRequestHandler<ObterAulasPorTurmasComponentesTipoCalendarioBimestresQuery, IEnumerable<TurmaComponenteDataAulaQuantidadeDto>>
    {
        private readonly IRegistroFrequenciaRepository registroFrequenciaRepository;

        public ObterAulasPorTurmasComponentesTipoCalendarioBimestresQueryHandler(IRegistroFrequenciaRepository registroFrequenciaRepository)
        {
            this.registroFrequenciaRepository = registroFrequenciaRepository ?? throw new ArgumentNullException(nameof(registroFrequenciaRepository));
        }
        public async Task<IEnumerable<TurmaComponenteDataAulaQuantidadeDto>> Handle(ObterAulasPorTurmasComponentesTipoCalendarioBimestresQuery request, CancellationToken cancellationToken)
            => await registroFrequenciaRepository.ObterAulasPorTurmasComponentesTipoCalendarioBimestres(request.TurmasCodigo, request.ComponentesCurricularesId, request.TipoCalendarioId, request.Bimestres);
    }
}
