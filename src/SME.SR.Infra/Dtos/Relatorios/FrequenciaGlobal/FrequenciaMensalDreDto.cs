using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class FrequenciaMensalDreDto
    {
        public FrequenciaMensalDreDto()
        {
            Ues = new List<FrequenciaMensalUeDto>();
        }
        public string CodigoDre { get; set; }
        public string NomeDre { get; set; }
        public List<FrequenciaMensalUeDto> Ues { get; set; }
    }
}