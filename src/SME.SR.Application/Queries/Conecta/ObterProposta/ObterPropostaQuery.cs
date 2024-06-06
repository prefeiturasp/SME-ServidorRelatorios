using MediatR;
using SME.SR.Data.Models.Conecta;

namespace SME.SR.Application
{
    public class ObterPropostaQuery : IRequest<Proposta>
    {
        public ObterPropostaQuery(long propostaId)
        {
            PropostaId = propostaId;
        }

        public long PropostaId { get; set; }
    }
}
