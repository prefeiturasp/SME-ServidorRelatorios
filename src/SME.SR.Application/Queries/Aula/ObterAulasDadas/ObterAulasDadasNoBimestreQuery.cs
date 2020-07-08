using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Application
{
    public class ObterAulasDadasNoBimestreQuery: IRequest<int>
    {
        public ObterAulasDadasNoBimestreQuery(string turmaCodigo, long tipoCalendarioId, long componenteCurricularCodigo, int bimestre)
        {
            TurmaCodigo = turmaCodigo;
            TipoCalendarioId = tipoCalendarioId;
            ComponenteCurricularCodigo = componenteCurricularCodigo;
            Bimestre = bimestre;
        }

        public string TurmaCodigo { get; set; }
        public long ComponenteCurricularCodigo { get; set; }
        public long TipoCalendarioId { get; set; }
        public int Bimestre { get; set; }
    }
}
