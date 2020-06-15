using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.Dtos
{
    public class TrabalhoParametrosDto
    {
        [JsonProperty("parameterValues"), AliasAs("parameterValues")]
        public IDictionary<string, IEnumerable<string>> Parametros { get; set; }
    }
}
