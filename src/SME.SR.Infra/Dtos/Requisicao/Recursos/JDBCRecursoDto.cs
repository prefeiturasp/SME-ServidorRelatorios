using Newtonsoft.Json;

namespace SME.SR.Infra.Dtos
{
    public class JDBCRecursoDto : DetalhesRecursoDto
    {
        [JsonProperty("driverClass")]
        public string ClasseDriver { get; set; }

        [JsonProperty("password")]
        public string Senha { get; set; }

        [JsonProperty("username")]
        public string Usuario { get; set; }

        [JsonProperty("connectionUrl")]
        public string UrlConexao { get; set; }

        [JsonProperty("timezone")]
        public string FusoHorario { get; set; }
    }
}
