using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioFrequenciaIndividualBimestresDto
    {
        public RelatorioFrequenciaIndividualBimestresDto()
        {
            Justificativas = new List<RelatorioFrequenciaIndividualJustificativasDto>();
        }
        public string NomeBimestre { get; set; }
        public RelatorioFrequenciaIndividualDadosFrequenciasDto DadosFrequencia { get; set; }
        public List<RelatorioFrequenciaIndividualJustificativasDto> Justificativas { get; set; }
    }
}

