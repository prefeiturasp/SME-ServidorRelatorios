using Newtonsoft.Json;

namespace SME.SR.Data
{
    public class ComponenteComNotaBimestre : ComponenteComNota
    {
        [JsonProperty("NotaConceito")]
        public string NotaConceito { get; set; }

        [JsonProperty("NotaPosConselho")]
        public string NotaPosConselho { get; set; }

        [JsonProperty("Aulas")]
        public int? Aulas { get; set; }
    }
}
