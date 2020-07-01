using Newtonsoft.Json;

namespace SME.SR.Infra.Dtos.Relatorios.HistoricoEscolar
{
    public class InformacoesAlunoDto
    {
        [JsonProperty("nome")]
        public string nome { get; set; }
        [JsonProperty("rga")]
        public string rga { get; set; }
        [JsonProperty("cidadeNatal")]
        public string cidadeNatal { get; set; }
        [JsonProperty("estadoNatal")]
        public string estadoNatal { get; set; }
        [JsonProperty("nacionalidade")]
        public string nacionalidade { get; set; }
        [JsonProperty("nascimento")]
        public string nascimento { get; set; }
        [JsonProperty("rg")]
        public string rg { get; set; }
        [JsonProperty("expedicao")]
        public string expedicao { get; set; }
        [JsonProperty("orgaoExpeditor")]
        public string orgaoExpeditor { get; set; }
        [JsonProperty("estado")]
        public string estado { get; set; }

    }
}
