using Newtonsoft.Json;

namespace SME.SR.Data
{
    public class ComponenteComNotaBimestre : ComponenteComNota
    {
        [JsonProperty("notaConceito")]
        public string NotaConceito { get; set; }

        [JsonProperty("notaPosConselho")]
        public string NotaPosConselho { get; set; }

        [JsonProperty("aulas")]
        public int? Aulas { get; set; }
    }
}
