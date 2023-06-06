using System;
using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioListagemRegistrosItineranciaDto
    {
        public string Dre { get; set; }
        public string Ue { get; set; }
        public string Usuario { get; set; }
        public DateTime DataSolicitacao { get; set; }
        public IEnumerable<RegistroListagemItineranciaDto> Registros { get; set; }        
    }
}
