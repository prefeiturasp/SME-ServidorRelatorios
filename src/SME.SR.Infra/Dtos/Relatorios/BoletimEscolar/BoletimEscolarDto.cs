using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SME.SR.Infra
{
    public class BoletimEscolarDto
    {
        [JsonProperty("boletins")]
        public List<BoletimEscolarAlunoDto> Boletins { get; set; }

        public BoletimEscolarDto()
        {
            Boletins = new List<BoletimEscolarAlunoDto>();
        }

        public BoletimEscolarDto(List<BoletimEscolarAlunoDto> boletins)
        {
            if (boletins != null && boletins.Any())
                this.Boletins = boletins;
        }
    }
}
