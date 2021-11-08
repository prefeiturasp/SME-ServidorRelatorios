using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.Dtos
{
    public class TurmaDadosPedagogicosDto
    {
        public string Nome { get; set; }
        public string SiglaModalidade { get; set; }
        public string ProfessorResponsavel { get; set; }
        public string ProfessorRF { get; set; }
        public int Aulas { get; set; }
        public int FrequenciasPendentes { get; set; }
        public DateTime DataUltimoRegistroFrequencia { get; set; }
        public int DiarioBordoPendentes { get; set; }
        public DateTime DataUltimoRegistroDiarioBordo { get; set; }
    }
}
