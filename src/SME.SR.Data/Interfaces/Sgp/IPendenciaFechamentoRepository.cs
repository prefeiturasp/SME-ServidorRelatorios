using SME.SR.Infra;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public interface IPendenciaFechamentoRepository
    {
        Task<IEnumerable<PendenciaParaFechamentoConsolidadoDto>> ObterPendenciasParaFechamentoConsolidado(string[] turmasCodigo, int[] bimestres, long[] componentesCurricularesId);
    }
}
