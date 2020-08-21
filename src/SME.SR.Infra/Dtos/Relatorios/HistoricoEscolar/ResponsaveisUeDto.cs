using Newtonsoft.Json;

namespace SME.SR.Infra
{
    public class ResponsaveisUeDto
    {
        [JsonProperty("nomeSecretario")]
        public string NomeSecretario { get; set; }

        [JsonProperty("documentoSecretario")]
        public string DocumentoSecretario { get; set; }

        [JsonProperty("nomeDiretor")]
        public string NomeDiretor { get; set; }

        [JsonProperty("documentoDiretor")]
        public string DocumentoDiretor { get; set; }
    }
}
