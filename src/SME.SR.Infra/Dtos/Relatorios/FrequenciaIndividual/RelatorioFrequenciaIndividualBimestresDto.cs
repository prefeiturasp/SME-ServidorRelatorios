using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioFrequenciaIndividualBimestresDto
    {
        public RelatorioFrequenciaIndividualBimestresDto()
        {
            FrequenciaDiaria = new List<RelatorioFrequenciaIndividualJustificativasDto>();
        }
        public string NomeBimestre { get; set; }
        public RelatorioFrequenciaIndividualDadosFrequenciasDto DadosFrequencia { get; set; }
        public List<RelatorioFrequenciaIndividualJustificativasDto> FrequenciaDiaria { get; set; }
        public bool ExibirFinal { get; set; }
    }
}

