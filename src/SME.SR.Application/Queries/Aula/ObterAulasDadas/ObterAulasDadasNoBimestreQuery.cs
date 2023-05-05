using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Application
{
    public class ObterAulasDadasNoBimestreQuery: IRequest<int>
    {
        public ObterAulasDadasNoBimestreQuery(string turmaCodigo, long tipoCalendarioId, long[] componenteCurricularCodigo, int bimestre, string professorTitularRf = null)
        {
            TurmaCodigo = turmaCodigo;
            TipoCalendarioId = tipoCalendarioId;
            ComponentesCurricularesCodigo = new string[] { componenteCurricularCodigo.ToString() };
            Bimestre = bimestre;
        }

        public ObterAulasDadasNoBimestreQuery(string turmaCodigo, long tipoCalendarioId, string[] componentesCurricularesCodigo, int bimestre, string professorTitularRf = null)
        {
            TurmaCodigo = turmaCodigo;
            TipoCalendarioId = tipoCalendarioId;
            ComponentesCurricularesCodigo = componentesCurricularesCodigo;
            Bimestre = bimestre;
            ProfessorTitularRf = professorTitularRf;
        }

        public string TurmaCodigo { get; set; }
        public string[] ComponentesCurricularesCodigo { get; set; }
        public long TipoCalendarioId { get; set; }
        public int Bimestre { get; set; }
        public string ProfessorTitularRf { get; set; }
    }
}
