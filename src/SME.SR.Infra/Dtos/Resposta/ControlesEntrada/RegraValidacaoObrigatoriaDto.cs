using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.Dtos
{
    public class RegraValidacaoObrigatoriaDto
    {
        [JsonProperty("errorMessage")]
        public string MensagemErro { get; set; }
    }
}
