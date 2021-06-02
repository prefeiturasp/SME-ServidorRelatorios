using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace SME.SR.Infra
{
    public class BoletimEscolarAlunoDto
    {
        [JsonProperty("descricaoGrupos")]
        public string DescricaoGrupos =>
            string.Join(" | ", Grupos.Select(x => $"{x.Nome}: {x.Descricao}").ToArray());

        [JsonProperty("tipoNota")]
        public string TipoNota { get; set; } = "Nota";

        [JsonProperty("cabecalho")]
        public BoletimEscolarCabecalhoDto Cabecalho { get; set; }

        [JsonProperty("parecerConclusivo")]
        public string ParecerConclusivo { get; set; }

        [JsonProperty("grupos")]
        public List<GrupoMatrizComponenteCurricularDto> Grupos { get; set; }

        public BoletimEscolarAlunoDto()
        {
            Cabecalho = new BoletimEscolarCabecalhoDto();
            Grupos = new List<GrupoMatrizComponenteCurricularDto>();
        }
    }
}
