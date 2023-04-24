using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterSecoesItineranciaEncaminhamentoNAAPAPorIdQuery : IRequest<IEnumerable<SecaoEncaminhamentoNAAPADto>>
    {
        public ObterSecoesItineranciaEncaminhamentoNAAPAPorIdQuery(long encaminhamentoNaapaId, string nomeComponenteSecao)
        {
            EncaminhamentoNaapaId = encaminhamentoNaapaId;
            NomeComponenteSecao = nomeComponenteSecao;
        }

        public long EncaminhamentoNaapaId { get; }
        public string NomeComponenteSecao { get; }
    }
}
