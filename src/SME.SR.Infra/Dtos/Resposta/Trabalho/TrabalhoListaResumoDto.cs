using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.Dtos
{
    public class TrabalhoListaResumoDto
    {
        [JsonProperty("jobsummary")]
        public IEnumerable<TrabalhoResumoDto> TrabalhoResumos { get; set; }
    }
}
