using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class FiltroRelatorioItineranciasDto
    {
        public IEnumerable<long> Itinerancias { get; set; }
        public string UsuarioNome { get; set; }
        public string UsuarioRF { get; set; }
    }
}
