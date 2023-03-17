using SME.SR.Infra;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data.Interfaces
{
    public interface IEncaminhamentoNAAPASecaoRepository
    {
        Task<IEnumerable<SecaoEncaminhamentoNAAPADto>> ObterSecoesPorEncaminhamentoIdAsync(long encaminhamentoNaapaId, string nomeComponenteSecao);
    }
}
