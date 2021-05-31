using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace SME.SR.Infra
{
   public class BoletimEscolarDetalhadoDto
    {
        [JsonProperty("boletins")]
        public List<BoletimEscolarAlunoDto> Boletins { get; set; }

        public BoletimEscolarDetalhadoDto()
        {
            Boletins = new List<BoletimEscolarAlunoDto>();
        }

        public BoletimEscolarDetalhadoDto(List<BoletimEscolarAlunoDto> boletins)
        {
            if (boletins != null && boletins.Any())
                this.Boletins = boletins;
        }
    }
}
