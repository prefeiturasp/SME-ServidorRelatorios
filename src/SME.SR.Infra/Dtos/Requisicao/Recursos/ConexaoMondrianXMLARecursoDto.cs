using Newtonsoft.Json;

namespace SME.SR.Infra.Dtos
{
    public class ConexaoMondrianXMLARecursoDto
    {
        [JsonProperty("mondrianConnectionReference")]
        public CaminhoArquivoRecursoDto CaminhoArquivo { get; set; }
    }
}
