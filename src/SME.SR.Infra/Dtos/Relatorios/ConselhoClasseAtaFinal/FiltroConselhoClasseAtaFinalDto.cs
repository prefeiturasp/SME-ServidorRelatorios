using System.Collections.Generic;
using static SME.SR.Infra.Enumeradores;

namespace SME.SR.Infra
{
    public class FiltroConselhoClasseAtaFinalDto
    {
        public IEnumerable<string> TurmasCodigos { get; set; }
        public TipoFormatoRelatorio TipoFormatoRelatorio { get; set; }
        public AtaFinalTipoVisualizacao? Visualizacao { get; set; }
        public int AnoLetivo { get; set; }
        public int Semestre { get; set; }
        public bool ImprimirComponentesQueNaoLancamNota { get; set; }
    }
}
