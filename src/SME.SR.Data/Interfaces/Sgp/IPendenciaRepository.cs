using SME.SR.Infra;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data.Interfaces
{
    public interface IPendenciaRepository
    {
        Task<IEnumerable<RelatorioPendenciasQueryRetornoDto>> ObterPendencias(int anoLetivo, string dreCodigo, string ueCodigo, long modalidadeId, int? semestre,
                                  string[] turmasCodigo, long[] componentesCodigo, int bimestre, bool pendenciaResolvida,int[] tipoPendenciaGrupo,string usuarioRf);
    }
}
