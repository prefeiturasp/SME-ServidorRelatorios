using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.Dtos.Resposta
{
    public class AnexoDto
    {
        [JsonProperty("contentType")]
        public string TipoConteudo { get; set; }

        [JsonProperty("fileName")]
        public string NomeArquivo { get; set; }
    }
}
