using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.Dtos
{
    public class TrabalhoOrigemDto
    {
        [JsonProperty("reportUnitURI"), AliasAs("reportUnitURI")]
        public string? CaminhoRelatorio { get; set; }

        [JsonProperty("parameters"), AliasAs("parameters")]
        public TrabalhoParametrosDto? Parametros { get; set; }
    }
}
