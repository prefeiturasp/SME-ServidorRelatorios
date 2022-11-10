using System.Collections.Generic;
using System.Threading.Tasks;
using SME.SR.Infra;

namespace SME.SR.Data.Interfaces
{
    public interface IPlanoAeeRespostaRepository
    {
        Task<IEnumerable<RespostaQuestaoDto>> ObterRespostasPorVersaoPlanoId(long versaoPlanoId);        
    }
}