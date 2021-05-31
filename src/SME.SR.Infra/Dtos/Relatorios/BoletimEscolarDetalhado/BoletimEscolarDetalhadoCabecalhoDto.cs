using Newtonsoft.Json;

namespace SME.SR.Infra
{
    public class BoletimEscolarDetalhadoCabecalhoDto
    {
        [JsonProperty("nomeDre")]
        public string NomeDre { get; set; }

        [JsonProperty("nomeUe")]
        public string NomeUe { get; set; }

        [JsonProperty("nomeTurma")]
        public string NomeTurma { get; set; }

        [JsonProperty("foto")]
        public string Foto { get; set; }

        [JsonProperty("aluno")]
        public string Aluno { get; set; }

        [JsonProperty("codigoEol")]
        public string CodigoEol { get; set; }

        [JsonProperty("data")]
        public string Data { get; set; }

        [JsonProperty("frequenciaGlobal")]
        public string FrequenciaGlobal { get; set; }

        [JsonProperty("ciclo")]
        public string Ciclo { get; set; }
    }
}
