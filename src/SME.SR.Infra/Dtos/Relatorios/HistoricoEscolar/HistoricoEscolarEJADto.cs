using Newtonsoft.Json;
using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class HistoricoEscolarEJADto
    {
        [JsonProperty("nomeDre")]
        public string NomeDre { get; set; }
        [JsonProperty("cabecalho")]
        public CabecalhoDto Cabecalho { get; set; }
        [JsonProperty("informacoesAluno")]
        public InformacoesAlunoDto InformacoesAluno { get; set; }
        [JsonProperty("ciclos")]
        public List<CicloDto> Ciclos { get; set; }
        [JsonProperty("tipoNota")]
        public TiposNotaEJADto TipoNota { get; set; }
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

        [JsonProperty("pareceresConclusivos")]
        public ParecerConclusivoEJADto ParecerConclusivo { get; set; }

        [JsonProperty("legenda")]
        public LegendaDto Legenda { get; set; }
        public Modalidade Modalidade { get; set; }

        [JsonProperty("responsaveisUe")]
        public ResponsaveisUeDto ResponsaveisUe { get; set; }

        [JsonProperty("dadosData")]
        public DadosDataDto DadosData { get; set; }
    }
}
