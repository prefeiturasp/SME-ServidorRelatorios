using Newtonsoft.Json;

namespace SME.SR.Infra.Dtos
{
    public class ConexaoOlapRecursoDto
    {
        [JsonProperty("olapConnectionReference")]
        public CaminhoArquivoRecursoDto CaminhoArquivo { get; set; }
    }
}
