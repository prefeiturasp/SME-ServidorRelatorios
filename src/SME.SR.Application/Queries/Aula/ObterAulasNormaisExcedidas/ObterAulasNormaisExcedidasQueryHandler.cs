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
    class ObterAulasNormaisExcedidasQueryHandler : IRequestHandler<ObterAulasNormaisExcedidasQuery, IEnumerable<AulaNormalExcedidoControleGradeDto>>
    {
        private readonly IAulaRepository aulaRepository;

        public ObterAulasNormaisExcedidasQueryHandler(IAulaRepository aulaRepository)
        {
            this.aulaRepository = aulaRepository ?? throw new ArgumentNullException(nameof(aulaRepository));
        }

        public async Task<IEnumerable<AulaNormalExcedidoControleGradeDto>> Handle(ObterAulasNormaisExcedidasQuery request, CancellationToken cancellationToken)
            => await aulaRepository.ObterAulasExcedidas(request.TurmaId,
                                                    request.ComponenteCurricularCodigo.ToString(),
                                                    request.TipoCalendarioId,
                                                    request.Bimestre);
    }
}
