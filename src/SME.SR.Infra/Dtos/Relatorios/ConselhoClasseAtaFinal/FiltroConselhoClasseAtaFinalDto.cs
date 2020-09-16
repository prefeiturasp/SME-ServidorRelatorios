using System.Collections.Generic;
using static SME.SR.Infra.Enumeradores;

namespace SME.SR.Infra
{
    public class FiltroConselhoClasseAtaFinalDto
    {
        public IEnumerable<string> TurmasCodigos { get; set; }

        public TipoFormatoRelatorio TipoFormatoRelatorio { get; set; }
    }
}
