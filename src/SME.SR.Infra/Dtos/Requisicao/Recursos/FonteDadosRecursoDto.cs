using Newtonsoft.Json;

namespace SME.SR.Infra.Dtos
{
    public class FonteDadosRecursoDto
    {
        [JsonProperty("dataSourceReference")]
        public CaminhoArquivoRecursoDto CaminhoArquivo { get; set; }
    }
}
