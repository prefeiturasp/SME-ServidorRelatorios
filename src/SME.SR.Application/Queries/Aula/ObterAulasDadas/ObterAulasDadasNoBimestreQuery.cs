using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Application
{
    public class ObterAulasDadasNoBimestreQuery: IRequest<int>
    {
        public ObterAulasDadasNoBimestreQuery(string turmaCodigo, long tipoCalendarioId, long[] componentesCurricularesCodigo, int bimestre)
        {
            TurmaCodigo = turmaCodigo;
            TipoCalendarioId = tipoCalendarioId;
            ComponentesCurricularesCodigo = componentesCurricularesCodigo;
            Bimestre = bimestre;
        }

        public string TurmaCodigo { get; set; }
        public long[] ComponentesCurricularesCodigo { get; set; }
        public long TipoCalendarioId { get; set; }
        public int Bimestre { get; set; }
    }
}
