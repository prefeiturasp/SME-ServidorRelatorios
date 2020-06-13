using Newtonsoft.Json;
using Refit;

namespace SME.SR.Infra.Dtos 
{ 
    public class TrabalhoGatilhoDto
    {
        [JsonProperty("simpleTrigger"), AliasAs("simpleTrigger")]
        public TrabalhoGatilhoSimplesDto GatilhoSimples { get; set; }
    }
}
