using Newtonsoft.Json;
using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class HistoricoEscolarEJANotasFrequenciaDto
    {
        [JsonProperty("baseNacionalComum")]
        public BaseNacionalComumEJADto BaseNacionalComum { get; set; }
        [JsonProperty("gruposComponentesCurriculares")]
        public List<GruposComponentesCurricularesEJADto> GruposComponentesCurriculares { get; set; }
        [JsonProperty("ensinoReligioso")]
        public EnsinoReligiosoEJADto EnsinoReligioso { get; set; }
        [JsonProperty("enriquecimentoCurricular")]
        public List<ComponenteCurricularHistoricoEscolarEJADto> EnriquecimentoCurricular { get; set; }

        [JsonProperty("projetosAtividadesComplementares")]
        public List<ComponenteCurricularHistoricoEscolarEJADto> ProjetosAtividadesComplementares { get; set; }
        [JsonProperty("tipoNota")]
        public TiposNotaEJADto TipoNota { get; set; }
        [JsonProperty("pareceresConclusivos")]
        public ParecerConclusivoEJADto ParecerConclusivo { get; set; }
    }
}
