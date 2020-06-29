using Newtonsoft.Json;
using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class ConselhoClasseAtaFinalPaginaDto : ConselhoClasseAtaFinalDto
    {
        public int NumeroPagina { get; set; }

        public int TotalPaginas { get; set; }

        public bool FinalHorizontal { get; set; }
    }
}
