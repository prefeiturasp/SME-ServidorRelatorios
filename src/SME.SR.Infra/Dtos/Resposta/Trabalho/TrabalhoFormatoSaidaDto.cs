using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.Dtos
{
    public class TrabalhoFormatoSaidaDto
    {
        [JsonProperty("outputFormat"), AliasAs("outputFormat")]
        public IEnumerable<string> FormatosSaida { get; set; }
    }
}
