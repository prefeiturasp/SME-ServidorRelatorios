using SME.SR.Data.Models;
using SME.SR.Infra;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data.Interfaces
{
    public interface IParametroSistemaRepository
    {
        Task<string> ObterValorPorTipo(TipoParametroSistema tipo);

        Task<IEnumerable<MediaFrequencia>> ObterMediasFrequencia();
        Task<string> ObterValorPorAnoTipo(int ano, TipoParametroSistema tipo);
    }
}
