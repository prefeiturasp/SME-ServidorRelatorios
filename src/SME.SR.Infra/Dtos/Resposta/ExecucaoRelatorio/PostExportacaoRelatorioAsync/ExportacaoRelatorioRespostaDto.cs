using Newtonsoft.Json;

namespace SME.SR.Infra.Dtos.Resposta
{
    public class ExportacaoRelatorioRespostaDto
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("outputResource")]
        public SaidaRecursoDto SaidaRecurso { get; set; }
    }
}
