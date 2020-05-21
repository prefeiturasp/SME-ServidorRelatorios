using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.Dtos
{
    public class TrabalhoListaIdsDto
    {
        [JsonProperty("jobId"), AliasAs("jobId")]
        public IEnumerable<int> TrabalhoIds { get; set; }
    }
}
