using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.Dtos
{
    public class RegraValidacaoFormatoDataHoraDto
    {
        [JsonProperty("errorMessage")]
        public string MenssagemErro { get; set; }

        [JsonProperty("format")]
        public string Formato { get; set; }
    }
}
