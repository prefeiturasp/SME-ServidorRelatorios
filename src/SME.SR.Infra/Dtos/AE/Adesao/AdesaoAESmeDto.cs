using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class AdesaoAESmeDto
    {
        public AdesaoAESmeDto()
        {
            Dres = new List<AdesaoAEDreDto>();
        }
        public AdesaoAEValoresDto Valores { get; set; }
        public List<AdesaoAEDreDto> Dres { get; set; }
    }
}