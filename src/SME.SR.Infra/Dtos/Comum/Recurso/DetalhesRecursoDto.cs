using Newtonsoft.Json;
using System;

namespace SME.SR.Infra.Dtos
{
    public class DetalhesRecursoDto
    {
        [JsonProperty("uri")]
        public string Diretorio { get; set; }

        [JsonProperty("label")]
        public string Titulo { get; set; }

        [JsonProperty("description")]
        public string Descricao { get; set; }

        [JsonProperty("permissionMask")]
        public string MascaraPermissao { get; set; }

        [JsonProperty("creationDate")]
        public DateTimeOffset DataCriacao { get; set; }

        [JsonProperty("updateDate")]
        public DateTimeOffset DataAtualizacao { get; set; }

        [JsonProperty("version")]
        public string Versao { get; set; }
    }
}
