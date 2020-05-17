using Newtonsoft.Json;

namespace SME.SR.Infra.Dtos
{
    public class ConexaoXMLARecursoDto : DetalhesRecursoDto
    {
        [JsonProperty("url")]
        public string UrlServicoXML { get; set; }

        [JsonProperty("xmlaDataSource")]
        public string FonteDadosXMLA { get; set; }

        [JsonProperty("catalog")]
        public string Catalogo { get; set; }

        [JsonProperty("username")]
        public string Usuario { get; set; }

        [JsonProperty("locale")]
        public string Senha { get; set; }
    }
}
