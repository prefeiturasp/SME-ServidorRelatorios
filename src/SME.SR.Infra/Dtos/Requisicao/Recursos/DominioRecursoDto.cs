using Newtonsoft.Json;

namespace SME.SR.Infra.Dtos
{
    public class DominioRecursoDto : DetalhesRecursoDto
    {
        [JsonProperty("dataSource")]
        public FonteDadosRecursoDto FonteDados { get; set; }

        [JsonProperty("schema")]
        public PadraoRecursoDto Padrao { get; set; }

        [JsonProperty("bundles")]
        public PacotesArquivosRecursoDto[] PacotesArquivos { get; set; }

        [JsonProperty("securityFile")]
        public ArquivoSegurancaRecursoDto ArquivoSeguranca { get; set; }
    }
}
