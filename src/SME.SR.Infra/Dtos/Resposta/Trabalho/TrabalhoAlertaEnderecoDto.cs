using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.Dtos
{
    public class TrabalhoAlertaEnderecoDto
    {
        [JsonProperty("address"), AliasAs("address")]
        public IList<string> Enderecos { get; set; }
    }
}
