using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.Dtos.Requisicao.Recursos
{
    public class TipoInformacaoRecursoDto : DetalhesRecursoDto
    {
        [JsonProperty("pattern")]
        public string Padrao { get; set; }

        [JsonProperty("maxValue")]
        public string ValorMaximo { get; set; }

        [JsonProperty("strictMax")]
        public bool RestringirMaximo { get; set; }

        [JsonProperty("minValue")]
        public string ValorMinimo { get; set; }

        [JsonProperty("strictMin")]
        public bool RestringirMinimo { get; set; }

        [JsonProperty("maxLength")]
        public string TamanhoMaximo { get; set; }
    }
}
