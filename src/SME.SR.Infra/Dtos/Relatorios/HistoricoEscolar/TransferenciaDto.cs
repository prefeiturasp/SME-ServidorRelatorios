using Newtonsoft.Json;
using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class TransferenciaDto
    {
        [JsonProperty("data")]
        public string Data { get; set; }

        [JsonProperty("descricao")]
        public string Descricao { get; set; }

        [JsonProperty("rodape")]
        public string Rodape { get; set; }

        [JsonIgnore]
        public string CodigoAluno { get; set; }

        [JsonIgnore]
        public string CodigoTurma { get; set; }

        [JsonProperty("baseNacionalComum")]
        public BaseNacionalComumTransferenciaDto BaseNacionalComum { get; set; }

        [JsonProperty("gruposComponentesCurriculares")]
        public List<GruposComponentesCurricularesTransferenciaDto> GruposComponentesCurriculares { get; set; }

        [JsonProperty("ensinoReligioso")]
        public EnsinoReligiosoTransferenciaDto EnsinoReligioso { get; set; }

        [JsonProperty("enriquecimentoCurricular")]
        public List<ComponenteCurricularHistoricoEscolarTransferenciaDto> EnriquecimentoCurricular { get; set; }

        [JsonProperty("projetosAtividadesComplementares")]
        public List<ComponenteCurricularHistoricoEscolarTransferenciaDto> ProjetosAtividadesComplementares { get; set; }
    }
}
