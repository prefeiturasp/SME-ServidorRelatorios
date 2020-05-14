using Newtonsoft.Json;

namespace SME.SR.Infra.Dtos.Requisicao.ExecucaoRelatorio.PostExecucaoRelatorioAsync
{
    public class ParametroDto
    {
        [JsonProperty("@name")]
        public string Name { get; set; }

        [JsonProperty("value")]
        public string[] Value { get; set; }
    }
}
