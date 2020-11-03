using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    class ObterQuantidadeAulasCriadasPeriodoInicioEFimQueryHandler : IRequestHandler<ObterQuantidadeAulasCriadasPeriodoInicioEFimQuery, int>
    {
        private readonly IAulaRepository aulaRepository;

        public ObterQuantidadeAulasCriadasPeriodoInicioEFimQueryHandler(IAulaRepository aulaRepository)
        {
            this.aulaRepository = aulaRepository ?? throw new ArgumentNullException(nameof(aulaRepository));
        }

        public async Task<int> Handle(ObterQuantidadeAulasCriadasPeriodoInicioEFimQuery request, CancellationToken cancellationToken)
        {
            return await aulaRepository.ObterQuantidadeAulaCriadasPeriodoInicioEFim(request.TurmaId, request.ComponenteCurricularId, 
                request.DataInicio, request.DataFim);
        }
            
    }
}
