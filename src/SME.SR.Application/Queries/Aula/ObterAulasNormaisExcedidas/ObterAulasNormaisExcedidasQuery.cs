using MediatR;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Application
{
    public class ObterAulasNormaisExcedidasQuery : IRequest<IEnumerable<AulasNormaisExcedidoControleGradeSinteticoDto>>
    {
        public ObterAulasNormaisExcedidasQuery(long turmaId, long tipoCalendarioId, long componenteCurricularCodigo, int bimestre)
        {
            TurmaId = turmaId;
            TipoCalendarioId = tipoCalendarioId;
            ComponenteCurricularCodigo = componenteCurricularCodigo;
            Bimestre = bimestre;
        }

        public long TurmaId { get; set; }
        public long ComponenteCurricularCodigo { get; set; }
        public long TipoCalendarioId { get; set; }
        public int Bimestre { get; set; }
    }
}
