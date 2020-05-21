using Newtonsoft.Json;

namespace SME.SR.Infra.Dtos
{
    public class PacotesArquivosRecursoDto
    {
        [JsonProperty("locale")]
        public string Localizacao { get; set; }

        [JsonProperty("file")]
        public PacoteArquivoRecursoDto Arquivo { get; set; }
    }
}
