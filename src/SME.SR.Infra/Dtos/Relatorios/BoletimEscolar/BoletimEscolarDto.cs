using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
    }
}
