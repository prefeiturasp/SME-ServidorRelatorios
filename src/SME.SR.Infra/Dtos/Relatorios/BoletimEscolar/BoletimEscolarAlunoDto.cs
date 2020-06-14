using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class BoletimEscolarAlunoDto
    {
        [JsonProperty("descricaoGrupos")]
        public string DescricaoGrupos { get; set; }

        [JsonProperty("cabecalho")]
        public BoletimEscolarCabecalhoDto Cabecalho { get; set; }

        [JsonProperty("grupos")]
        public List<GrupoMatrizComponenteCurricularDto> Grupos { get; set; }

        public BoletimEscolarAlunoDto()
        {
            Cabecalho = new BoletimEscolarCabecalhoDto();
            Grupos = new List<GrupoMatrizComponenteCurricularDto>();
        }
    }
}
