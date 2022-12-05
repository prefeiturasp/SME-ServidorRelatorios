using System.Collections.Generic;
using System.Threading.Tasks;
using SME.SR.Infra;

namespace SME.SR.Data.Interfaces
{
    public interface IPlanoAeeVersaoRepository
    {
        Task<PlanoAeeDto> ObterPlanoAeePorVersaoPlanoId(long versaoPlanoId);
        Task<IEnumerable<PlanosAeeDto>> ObterPlanoAEE(FiltroRelatorioPlanosAeeDto requestFiltro);
    }
}