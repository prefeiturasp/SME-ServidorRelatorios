using SME.SR.Data.Models.Conecta;
using System.Threading.Tasks;

namespace SME.SR.Data.Interfaces
{
    public interface IPropostaRepository
    {
        Task<Proposta> ObterProposta(long propostaId);
        Task<PropostaCompleta> ObterPropostaCompleta(long propostaId);
    }
}
