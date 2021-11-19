using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterOcorrenciasPorCodigoETurmaQuery : IRequest<IEnumerable<OcorrenciasPorCodigoTurmaDto>>
    {
        public ObterOcorrenciasPorCodigoETurmaQuery(string turmaCodigo, long[] ocorrenciaIds)
        {
            TurmaCodigo = turmaCodigo;
            OcorrenciaIds = ocorrenciaIds;
        }

        public string TurmaCodigo { get; set; }
        public long[] OcorrenciaIds { get; set; }
    }
}
