using Newtonsoft.Json;
using System;

namespace SME.SR.Infra
{
    public class InformacoesAlunoDto
    {
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

        [JsonIgnore]
        public SituacaoMatriculaAluno CodigoSituacaoMatricula { get; set; }
        [JsonIgnore]
        public string SituacaoMatricula { get; set; }
        [JsonIgnore]
        public DateTime DataSituacao { get; set; }
        [JsonIgnore]
        public bool Ativo { get; set; }
        [JsonIgnore]
        public string DataSituacaoFormatada => Ativo ? DateTime.Now.ToString("dd/MM/yyyy") : DataSituacao.ToString("dd/MM/yyyy");

    }
}