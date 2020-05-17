using Newtonsoft.Json;

namespace SME.SR.Infra.Dtos
{
    public class ArquivoSegurancaRecursoDto
    {
        [JsonProperty("securityFileReference")]
        public CaminhoArquivoRecursoDto CaminhoArquivo { get; set; }
    }
}
