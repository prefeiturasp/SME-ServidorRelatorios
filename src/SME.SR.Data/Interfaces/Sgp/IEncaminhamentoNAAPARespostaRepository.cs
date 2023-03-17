using SME.SR.Infra;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data.Interfaces
{
    public interface IEncaminhamentoNAAPARespostaRepository
    {
        Task<IEnumerable<RespostaQuestaoDto>> ObterRespostasPorEncaminhamentoIdAsync(long encaminhamentoNaapaId, string nomeComponenteSecao);
    }
}
