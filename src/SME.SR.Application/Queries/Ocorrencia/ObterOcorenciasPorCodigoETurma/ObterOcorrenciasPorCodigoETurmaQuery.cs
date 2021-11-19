using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterOcorrenciasPorCodigoETurmaQuery : IRequest<IEnumerable<OcorrenciasPorCodigoTurmaDto>>
    {
        public ObterOcorrenciasPorCodigoETurmaQuery(long turmaId, long[] ocorrenciaIds)
        {
            TurmaId = turmaId;
            OcorrenciaIds = ocorrenciaIds;
        }

        public long TurmaId { get; set; }
        public long[] OcorrenciaIds { get; set; }
    }
}
