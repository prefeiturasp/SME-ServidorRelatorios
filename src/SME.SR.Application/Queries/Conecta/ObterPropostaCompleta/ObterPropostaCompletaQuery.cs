using MediatR;
using SME.SR.Data.Models.Conecta;

namespace SME.SR.Application
{
    public class ObterPropostaCompletaQuery : IRequest<PropostaCompleta>
    {
        public ObterPropostaCompletaQuery(long propostaId)
        {
            PropostaId = propostaId;
        }

        public long PropostaId { get; set; }
    }
}
