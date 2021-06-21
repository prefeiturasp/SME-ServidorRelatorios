using SME.SR.Infra;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public interface IComponenteCurricularGrupoAreaOrdenacaoRepository
    {
        Task<IEnumerable<ComponenteCurricularGrupoAreaOrdenacaoDto>> ObterOrdenacaoPorGruposAreas(long[] grupoMatrizIds, long[] areaConhecimentoIds);
    }
}
