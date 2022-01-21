using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class ComponenteCurricularPedagogicoDto
    {
        public string NomeComponente { get; set; }
        public string ProfessorResponsavel { get; set; }
        public string ProfessorRF { get; set; }
        public int Aulas { get; set; }
        public int FrequenciasPendentes { get; set; }
        public DateTime DataUltimoRegistroFrequencia { get; set; }
        public int PlanosAulaPendentes { get; set; }
        public DateTime DataUltimoRegistroPlanoAula { get; set; }
    }
}
