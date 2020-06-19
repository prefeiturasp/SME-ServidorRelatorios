using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace SME.SR.Infra
{
    public class BoletimEscolarAlunoDto
    {
        [JsonProperty("descricaoGrupos")]
        public string DescricaoGrupos { get; set; }

        [JsonProperty("tipoNota")]
        public string TipoNota { get; private set; } = "Nota";

        public void SetarTipoNota(string notaConceito)
        {
            if (!string.IsNullOrEmpty(notaConceito))
            {
                if (double.TryParse(notaConceito, out _))
                    TipoNota = "Nota";
                else if (notaConceito.Equals("NF") ||
                        notaConceito.Equals("F"))
                    TipoNota = "Sintese";
                else
                    TipoNota = "Conceito";
            }
        }

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
