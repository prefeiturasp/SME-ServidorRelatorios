using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterMapeamentosEstudantesPorFiltroQueryHandler : IRequestHandler<ObterMapeamentosEstudantesPorFiltroQuery, IEnumerable<MapeamentoEstudanteUltimoBimestreDto>>
    {
        private readonly IMapeamentoEstudanteRepository mapeamentoRepository;

        public ObterMapeamentosEstudantesPorFiltroQueryHandler(IMapeamentoEstudanteRepository mapeamentoRepository)
        {
            this.mapeamentoRepository = mapeamentoRepository ?? throw new ArgumentNullException(nameof(mapeamentoRepository));
        }

        public Task<IEnumerable<MapeamentoEstudanteUltimoBimestreDto>> Handle(ObterMapeamentosEstudantesPorFiltroQuery request, CancellationToken cancellationToken)
        {
            return mapeamentoRepository.ObterMapeamentosEstudantesFiltro(request.Filtro);   
        }
    }
}
