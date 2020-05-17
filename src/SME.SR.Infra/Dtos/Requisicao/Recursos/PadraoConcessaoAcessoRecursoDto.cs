using Newtonsoft.Json;

namespace SME.SR.Infra.Dtos
{
    public class PadraoConcessaoAcessoRecursoDto
    {
        [JsonProperty("accessGrantSchemaReference")]
        public CaminhoArquivoRecursoDto CaminhoArquivo { get; set; }
    }
}
