using SME.SR.Infra.Dtos.AE.Adesao;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public interface IDashboardAdesaoRepository
    {
        Task<IEnumerable<AdesaoAEQueryConsolidadoRetornoDto>> ObterAdesaoDashboardPorFiltros(string dreCodigo, string ueCodigo);
    }
}
