using Newtonsoft.Json;

namespace SME.SR.Infra.Dtos
{
    public class PacoteArquivoRecursoDto
    {
        [JsonProperty("fileReference")]
        public CaminhoArquivoRecursoDto CaminhoArquivo { get; set; }
    }
}
