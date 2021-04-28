using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterComponenteCurricularGrupoAreaOrdenacaoQuery : IRequest<IEnumerable<ComponenteCurricularGrupoAreaOrdenacaoDto>>
    {
        public ObterComponenteCurricularGrupoAreaOrdenacaoQuery(long[] grupoMatrizIds, long[] areaDoConhecimentoIds)
        {
            GrupoMatrizIds = grupoMatrizIds;
            AreaDoConhecimentoIds = areaDoConhecimentoIds;
        }

        public long[] GrupoMatrizIds { get; set; }

        public long[] AreaDoConhecimentoIds { get; set; }
    }
}
