using Newtonsoft.Json;
using System.Collections.Generic;

namespace SME.SR.Infra.Dtos.Relatorios.HistoricoEscolar
{
    public class HistoricoEscolarDTO
    {
        [JsonProperty("nomeDre")]
        public string nomeDre { get; set; }
        [JsonProperty("cabecalho")]
        public CabecalhoDto cabecalho { get; set; }
        [JsonProperty("informacoesAluno")]
        public InformacoesAlunoDto informacoesAluno { get; set; }
        [JsonProperty("ciclos")]
        public List<CicloDto> ciclos { get; set; }
        [JsonProperty("gruposComponentesCurriculares")]
        public List<GruposComponentesCurricularesDto> gruposComponentesCurriculares { get; set; }
        [JsonProperty("ensinoReligioso")]
        public EnsinoReligiosoDto ensinoReligioso { get; set; }
        [JsonProperty("enriquecimentoCurricular")]
        public List<EnriquecimentoCurricularDto> enriquecimentoCurricular { get; set; }
        [JsonProperty("parecerConclusivo")]
        public List<ParecerConclusivoDto> parecerConclusivo { get; set; }
        [JsonProperty("legenda")]
        public LegendaDto legenda { get; set; }
    }
}
