using MediatR;
using SME.SR.Data;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterPorCicloIdDataAvalicacaoQueryHandler : IRequestHandler<ObterPorCicloIdDataAvalicacaoQuery, NotaTipoValor>
    {
        private readonly ICicloRepository cicloRepository;

        public ObterPorCicloIdDataAvalicacaoQueryHandler(ICicloRepository cicloRepository)
        {
            this.cicloRepository = cicloRepository ?? throw new ArgumentNullException(nameof(cicloRepository));
        }

        public Task<NotaTipoValor> Handle(ObterPorCicloIdDataAvalicacaoQuery request, CancellationToken cancellationToken)
                => cicloRepository.ObterPorCicloIdDataAvalicacao(request.CicloId, request.DataAvalicao);
    }
}
