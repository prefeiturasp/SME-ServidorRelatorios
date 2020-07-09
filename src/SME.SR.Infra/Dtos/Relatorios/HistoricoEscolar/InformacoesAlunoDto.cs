using Newtonsoft.Json;

namespace SME.SR.Infra
{
    public class InformacoesAlunoDto
    {
        [JsonIgnore]
        public string Codigo { get; set; }
        [JsonProperty("nome")]
        public string Nome { get; set; }
        [JsonProperty("rga")]
        public string Rga { get; set; }
        [JsonProperty("cidadeNatal")]
        public string CidadeNatal { get; set; }
        [JsonProperty("estadoNatal")]
        public string EstadoNatal { get; set; }
        [JsonProperty("nacionalidade")]
        public string Nacionalidade { get; set; }
        [JsonProperty("nascimento")]
        public string Nascimento { get; set; }
        [JsonProperty("rg")]
        public string Rg { get; set; }
        [JsonProperty("expedicao")]
        public string Expedicao { get; set; }
        [JsonProperty("orgaoExpeditor")]
        public string OrgaoExpeditor { get; set; }
        [JsonProperty("estado")]
        public string Estado { get; set; }

        public string Codigo { get; set; }
    }
}