using Newtonsoft.Json;

namespace SME.SR.Infra.Dtos
{
    public class ControleEntradaQueryRecursoDto
    {
        [JsonProperty("queryReference")]
        public CaminhoArquivoRecursoDto CaminhoArquivo { get; set; }
    }
}
