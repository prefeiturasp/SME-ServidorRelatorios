using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioFrequenciaIndividualJustificativasDto
    {
        public string DataAula { get; set; }
        public long QuantidadeAulas { get; set; }
        public int QuantidadePresenca { get; set; }
        public int QuantidadeRemoto { get; set; }
        public long QuantidadeAusencia { get; set; }
        public string Justificativa { get; set; }
    }
}
