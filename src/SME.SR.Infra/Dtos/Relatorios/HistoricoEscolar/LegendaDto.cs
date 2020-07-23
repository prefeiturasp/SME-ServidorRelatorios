using Newtonsoft.Json;

namespace SME.SR.Infra
{
    public class LegendaDto
    {
        [JsonProperty("texto")]
        public string Texto { get; set; }

        public LegendaDto(string texto)
        {
            Texto = texto;
        }
    }
}
