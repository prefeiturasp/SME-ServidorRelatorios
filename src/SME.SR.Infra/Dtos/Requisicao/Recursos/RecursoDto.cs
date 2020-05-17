using Newtonsoft.Json;

namespace SME.SR.Infra.Dtos
{
    public class RecursoDto
    {
        [JsonProperty("name")]
        public CaminhoArquivoRecursoDto CaminhoArquivo { get; set; }

        [JsonProperty("file")]
        public string Conteudo { get; set; }

        [JsonProperty("fileReference")]
        public CaminhoArquivoRecursoDto ReferenciaArquivo { get; set; }
    }
}
