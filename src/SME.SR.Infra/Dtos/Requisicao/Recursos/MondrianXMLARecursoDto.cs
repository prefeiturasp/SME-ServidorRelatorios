using Newtonsoft.Json;

namespace SME.SR.Infra.Dtos
{
    public class MondrianXMLARecursoDto : DetalhesRecursoDto
    {
        [JsonProperty("catalog")]
        public string Catalogo { get; set; }

        [JsonProperty("mondrianConnection")]
        public ConexaoMondrianXMLARecursoDto Conexao { get; set; }
    }
}
