using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterSecoesItineranciaEncaminhamentoNAAPAPorIdQueryHandler : IRequestHandler<ObterSecoesItineranciaEncaminhamentoNAAPAPorIdQuery, IEnumerable<SecaoEncaminhamentoNAAPADto>>
    {
        private readonly IEncaminhamentoNAAPASecaoRepository encaminhamentoNAAPASecaoRepository;

        public ObterSecoesItineranciaEncaminhamentoNAAPAPorIdQueryHandler(IEncaminhamentoNAAPASecaoRepository encaminhamentoNAAPASecaoRepository)
        {
            this.encaminhamentoNAAPASecaoRepository = encaminhamentoNAAPASecaoRepository ?? throw new ArgumentNullException(nameof(encaminhamentoNAAPASecaoRepository));
        }

        public Task<IEnumerable<SecaoEncaminhamentoNAAPADto>> Handle(ObterSecoesItineranciaEncaminhamentoNAAPAPorIdQuery request, CancellationToken cancellationToken)
        {
            return encaminhamentoNAAPASecaoRepository.ObterSecoesPorEncaminhamentoIdAsync(request.EncaminhamentoNaapaId, request.NomeComponenteSecao);
        }
    }
}
