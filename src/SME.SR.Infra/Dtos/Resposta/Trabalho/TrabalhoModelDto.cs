using Newtonsoft.Json;

namespace SME.SR.Infra.Dtos
{
    public class TrabalhoModelDto
    {
        [JsonProperty("label")]
        public string? Texto { get; set; }

        [JsonProperty("isDescriptionModified")]
        public bool? DescricaoModificada { get; set; }

        [JsonProperty("triggerModel")]
        public TrabalhoGatilhoDto? Gatilho { get; set; }

        [JsonProperty("baseOutputFilename")]
        public string? NomeBaseArquivoSaida { get; set; }
    }
}
