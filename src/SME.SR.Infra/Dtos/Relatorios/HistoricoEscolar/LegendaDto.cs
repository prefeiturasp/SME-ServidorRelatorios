using Newtonsoft.Json;

namespace SME.SR.Infra
{
    public class LegendaDto
    {
        [JsonProperty("texto")]
        public string Texto { get; set; }
        public string TextoSintese { get; set; }
        public string TextoConceito { get; set; }

        public LegendaDto()
        {
        }
    }
}
