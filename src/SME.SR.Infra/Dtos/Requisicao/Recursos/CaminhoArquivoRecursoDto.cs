using Newtonsoft.Json;

namespace SME.SR.Infra.Dtos
{
    public class CaminhoArquivoRecursoDto
    {
        [JsonProperty("uri")]
        public string Caminho { get; set; }
    }
}
