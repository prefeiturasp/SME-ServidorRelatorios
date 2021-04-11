using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RegistrosRegistroItineranciaDto
    {
        public string DataVisita { get; set; }
        public IEnumerable<ObjetivosRegistroItineranciaDto> Objetivos { get; set; }
        public IEnumerable<SecoesRegistroItineranciaDto> Secoes { get; set; }
    }
}
