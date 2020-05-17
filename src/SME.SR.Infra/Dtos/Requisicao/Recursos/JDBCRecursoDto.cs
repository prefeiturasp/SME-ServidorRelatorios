using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.Dtos.Requisicao.Recursos
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
