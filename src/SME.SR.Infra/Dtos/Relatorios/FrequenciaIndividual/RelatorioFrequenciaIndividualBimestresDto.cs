using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioFrequenciaIndividualBimestresDto
    {
        public string NomeBimestre { get; set; }
        public int TotalAulasDadas { get; set; }
        public int TotalPresencas { get; set; }
        public int TotalRemoto { get; set; }
        public int TotalAusencias { get; set; }
        public int TotalCompensacoes { get; set; }
        public string PercentualFrequencia { get; set; }
        public List<RelatorioFrequenciaIndividualJustificativasDto> Justificativas { get; set; }
    }
}
