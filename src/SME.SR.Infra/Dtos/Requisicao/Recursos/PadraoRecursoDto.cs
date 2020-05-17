using Newtonsoft.Json;

namespace SME.SR.Infra.Dtos
{
    public class PadraoRecursoDto
    {
        [JsonProperty("schemaFileReference")]
        public CaminhoArquivoRecursoDto CaminhoArquivo { get; set; }
    }
}
