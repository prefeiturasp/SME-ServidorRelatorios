using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class FrequenciaMensalDreDto
    {
        public string CodigoDre { get; set; }
        public string NomeDre { get; set; }
        public IEnumerable<FrequenciaMensalUeDto> FrequenciaMensalUe { get; set; }
    }
}