using SME.SR.Infra;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public interface ICompensacaoAusenciaRepository
    {
        Task<IEnumerable<RelatorioCompensacaoAusenciaRetornoConsulta>> ObterPorUeModalidadeSemestreComponenteBimestre(long UeId, int modalidadeId, int? semestre, string turmaCodigo, long[] componetesCurricularesIds, int bimestre, int anoLetivo);
    }
}
