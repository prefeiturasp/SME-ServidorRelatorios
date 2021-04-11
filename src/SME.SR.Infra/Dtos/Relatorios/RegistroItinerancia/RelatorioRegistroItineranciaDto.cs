using System;
using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioRegistroItineranciaDto
    {
        public string Dre { get; set; }
        public string Ue { get; set; }
        public string Usuario { get; set; }
        public string RF { get; set; }
        public string DataSolicitacao { get; set; }
        public IEnumerable<RegistrosRegistroItineranciaDto> Registros { get; set; }        
    }
}
