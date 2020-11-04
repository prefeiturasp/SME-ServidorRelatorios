using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class DetalharAulasDuplicadasPorTurmaComponenteEBimestreQueryHandler : IRequestHandler<DetalharAulasDuplicadasPorTurmaComponenteEBimestreQuery, IEnumerable<AulaDuplicadaControleGradeDto>>
    {
        private readonly IAulaRepository aulaRepository;

        public DetalharAulasDuplicadasPorTurmaComponenteEBimestreQueryHandler(IAulaRepository aulaRepository)
        {
            this.aulaRepository = aulaRepository ?? throw new ArgumentNullException(nameof(aulaRepository));
        }

        public async Task<IEnumerable<AulaDuplicadaControleGradeDto>> Handle(DetalharAulasDuplicadasPorTurmaComponenteEBimestreQuery request, CancellationToken cancellationToken)
            => await aulaRepository.DetalharAulasDuplicadasNoDia(request.TurmaId,
                                                    request.ComponenteCurricularId.ToString(),
                                                    request.TipoCalendarioId,
                                                    request.Bimestre);
    }
}
