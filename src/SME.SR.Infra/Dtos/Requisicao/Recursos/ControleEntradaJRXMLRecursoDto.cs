using Newtonsoft.Json;

namespace SME.SR.Infra.Dtos
{
    public class ControleEntradaJRXMLRecursoDto
    {
        [JsonProperty("inputControlReference")]
        public CaminhoArquivoRecursoDto CaminhoArquivo { get; set; }
    }
}
