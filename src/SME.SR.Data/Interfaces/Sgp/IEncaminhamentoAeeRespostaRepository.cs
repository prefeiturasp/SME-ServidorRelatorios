using System.Collections.Generic;
using System.Threading.Tasks;
using SME.SR.Infra;

namespace SME.SR.Data.Interfaces
{
    public interface IEncaminhamentoAeeRespostaRepository
    {
        Task<IEnumerable<RespostaQuestaoDto>> ObterRespostasPorEncaminhamentoId(long encaminhamentoAeeId, string? nomeComponenteSecao);        
    }
}