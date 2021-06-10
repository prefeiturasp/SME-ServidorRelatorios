using Newtonsoft.Json;
using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class BoletimEscolarDetalhadoAlunoDto
    {
        [JsonProperty("tipoNota")]
        public string TipoNota { get; set; } = "Nota";

        [JsonProperty("cabecalho")]
        public BoletimEscolarDetalhadoCabecalhoDto Cabecalho { get; set; }

        [JsonProperty("componenteCurricularRegencia")]
        public ComponenteCurricularRegenciaDto ComponenteCurricularRegencia { get; set; }

        [JsonProperty("grupos")]
        public List<GrupoMatrizComponenteCurricularDto> Grupos { get; set; }

        [JsonProperty("parecerConclusivo")]
        public string ParecerConclusivo { get; set; }

        [JsonProperty("recomendacoesEstudante")]
        public string RecomendacoesEstudante { get; set; }

        [JsonProperty("recomendacoesFamilia")]
        public string RecomendacoesFamilia { get; set; }

        public BoletimEscolarDetalhadoAlunoDto()
        {
            Cabecalho = new BoletimEscolarDetalhadoCabecalhoDto();
            Grupos = new List<GrupoMatrizComponenteCurricularDto>();
        }
    }
}
