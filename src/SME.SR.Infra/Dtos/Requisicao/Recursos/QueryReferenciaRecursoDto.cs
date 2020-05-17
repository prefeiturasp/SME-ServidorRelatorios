using Newtonsoft.Json;

namespace SME.SR.Infra.Dtos
{
    public class QueryReferenciaRecursoDto
    {
        [JsonProperty("queryReference")]
        public CaminhoArquivoRecursoDto CaminhoArquivo { get; set; }
    }
}
