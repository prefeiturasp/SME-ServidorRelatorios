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
    class ObterDiasAulaCriadasPeriodoInicioEFimQueryHandler : IRequestHandler<ObterDiasAulaCriadasPeriodoInicioEFimQuery, int>
    {
        private readonly IAulaRepository aulaRepository;

        public ObterDiasAulaCriadasPeriodoInicioEFimQueryHandler(IAulaRepository aulaRepository)
        {
            this.aulaRepository = aulaRepository ?? throw new ArgumentNullException(nameof(aulaRepository));
        }

        public async Task<int> Handle(ObterDiasAulaCriadasPeriodoInicioEFimQuery request, CancellationToken cancellationToken)
        {
            var aulas = await aulaRepository.ObterDiasAulaCriadasPeriodoInicioEFim(request.TurmaId,
                                                    request.ComponenteCurricularId,
                                                    request.DataInicio,
                                                    request.DataFim);

            return aulas.Count();
        }
            
    }
}
