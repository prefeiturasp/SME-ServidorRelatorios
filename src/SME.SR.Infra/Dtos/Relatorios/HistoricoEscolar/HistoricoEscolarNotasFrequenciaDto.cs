using Newtonsoft.Json;
using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class HistoricoEscolarNotasFrequenciaDto
    {
        [JsonProperty("baseNacionalComum")]
        public BaseNacionalComumDto BaseNacionalComum { get; set; }
        [JsonProperty("gruposComponentesCurriculares")]
        public List<GruposComponentesCurricularesDto> GruposComponentesCurriculares { get; set; }
        [JsonProperty("ensinoReligioso")]
        public EnsinoReligiosoDto EnsinoReligioso { get; set; }
        [JsonProperty("enriquecimentoCurricular")]
        public List<ComponenteCurricularHistoricoEscolarDto> EnriquecimentoCurricular { get; set; }

        [JsonProperty("projetosAtividadesComplementares")]
        public List<ComponenteCurricularHistoricoEscolarDto> ProjetosAtividadesComplementares { get; set; }
        [JsonProperty("tipoNota")]
        public TiposNotaDto TipoNota { get; set; }
        [JsonProperty("pareceresConclusivos")]
        public ParecerConclusivoFrequenciaGlobalDto ParecerConclusivo { get; set; }
        [JsonProperty("frequenciaGlobal")]
        public ParecerConclusivoFrequenciaGlobalDto FrequenciaGlobal { get; set; }
    }
}
