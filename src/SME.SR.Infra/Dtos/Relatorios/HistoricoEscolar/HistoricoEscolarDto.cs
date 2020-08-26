using Newtonsoft.Json;
using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class HistoricoEscolarDTO
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
        public TiposNotaDto TipoNota { get; set; }
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

        [JsonProperty("pareceresConclusivos")]
        public ParecerConclusivoDto ParecerConclusivo { get; set; }

        [JsonProperty("legenda")]
        public LegendaDto Legenda { get; set; }
        public Modalidade Modalidade { get; set; }

        [JsonProperty("responsaveisUe")]
        public ResponsaveisUeDto ResponsaveisUe { get; set; }

        [JsonProperty("dadosData")]
        public DadosDataDto DadosData { get; set; }

        [JsonProperty("historicoEscolarTranferencia")]
        public TransferenciaDto DadosTransferencia{ get; set; }
    }
}
