using Newtonsoft.Json;

namespace SME.SR.Infra.Dtos
{
    public class JRXMLRecursoDto
    {
        [JsonProperty("jrxmlFileReference")]
        public CaminhoArquivoRecursoDto CaminhoArquivo { get; set; }

        [JsonProperty("jrxmlFile")]
        public ArquivoJRXMLRecursoDto Arquivo{ get; set; }
    }
}
