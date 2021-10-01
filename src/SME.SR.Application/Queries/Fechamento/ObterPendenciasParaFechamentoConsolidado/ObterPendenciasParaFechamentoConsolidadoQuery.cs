using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterPendenciasParaFechamentoConsolidadoQuery : IRequest<IEnumerable<PendenciaParaFechamentoConsolidadoDto>>
    {
        public ObterPendenciasParaFechamentoConsolidadoQuery(string[] turmasCodigo, int[] bimestres, long[] componentesCurricularesId)
        {
            TurmasCodigo = turmasCodigo;
            Bimestres = bimestres;
            ComponentesCurricularesId = componentesCurricularesId;
        }

        public string[] TurmasCodigo { get; set; }
        public int[] Bimestres { get; set; }
        public long[] ComponentesCurricularesId { get; set; }
    }
}
