using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class AdesaoAEDreDto
    {
        public AdesaoAEDreDto()
        {
            Ues = new List<AdesaoAEValoresDto>();
        }
        public AdesaoAEValoresDto Valores { get; set; }
        public List<AdesaoAEValoresDto> Ues { get; set; }
    }
}