using MediatR;
using SME.SR.Data;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterAtribuicoesEsporadicasPorFiltroQueryHandler : IRequestHandler<ObterAtribuicoesEsporadicasPorFiltroQuery, IEnumerable<AtribuicaoEsporadica>>
    {
        private readonly IAtribuicaoEsporadicaRepository atribuicaoEsporadicaRepository;

        public ObterAtribuicoesEsporadicasPorFiltroQueryHandler(IAtribuicaoEsporadicaRepository atribuicaoEsporadicaRepository)
        {
            this.atribuicaoEsporadicaRepository = atribuicaoEsporadicaRepository ?? throw new ArgumentNullException(nameof(atribuicaoEsporadicaRepository));
        }

        public async Task<IEnumerable<AtribuicaoEsporadica>> Handle(ObterAtribuicoesEsporadicasPorFiltroQuery request, CancellationToken cancellationToken)
        {
            var lstAtribuicoes = await atribuicaoEsporadicaRepository.ObterPorFiltros(request.AnoLetivo,
                                                        request.DreId,
                                                         request.UeId,
                                                         request.CodigoRF);

            return lstAtribuicoes;
        }
    }
}
