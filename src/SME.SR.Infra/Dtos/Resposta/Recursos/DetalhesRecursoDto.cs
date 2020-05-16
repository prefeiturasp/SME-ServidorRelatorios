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

        [JsonProperty("type")]
        public string Tipo { get; set; }

        [JsonProperty("permissionMask")]
        public long MascaraPermissao { get; set; }

        [JsonProperty("creationDate")]
        public DateTimeOffset DataCriacao { get; set; }

        [JsonProperty("updateDate")]
        public DateTimeOffset DataAtualizacao { get; set; }

        [JsonProperty("version")]
        public long Versao { get; set; }
    }
}
