using SME.SR.Infra;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public interface IUeRepository
    {
        Task<Ue> ObterPorCodigo(string ueCodigo);
        Task<IEnumerable<Ue>> ObterPorCodigos(string[] ueCodigos);
        Task<IEnumerable<UePorDresIdResultDto>> ObterPorDresId(long[] dreIds);
        Task<IEnumerable<Ue>> ObterPorDreSemestreModadalidadeAnoId(long dreId, int? semestre, int modalidadeId, string[] anos, IEnumerable<int> tipoEscolas);
        Task<Ue> ObterPorId(long ueId);
        Task<Ue> ObterUeComDrePorId(long ueId);
        Task<Ue> ObterUeComDrePorCodigo(string codigoUe);
    }
}
