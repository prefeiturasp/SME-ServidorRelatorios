using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.Dtos.Requisicao.Recursos
{
    public class AWSRecursoDto : DetalhesRecursoDto
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

        [JsonProperty("accessKey")]
        public string ChaveAcesso { get; set; }

        [JsonProperty("secretKey")]
        public string ChaveSegredo { get; set; }

        [JsonProperty("roleArn")]
        public string RegraArn { get; set; }

        [JsonProperty("region")]
        public string Regiao { get; set; }

        [JsonProperty("dbName")]
        public string NomeBancoDados { get; set; }

        [JsonProperty("dbInstanceIdentifier")]
        public string IdentificadorInstanciaBancoDados { get; set; }

        [JsonProperty("dbService")]
        public string ServicoBancoDados { get; set; }
    }
}
