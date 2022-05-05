using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class FrequenciaMensalDto
    {
        public FrequenciaMensalCabecalhoDto Cabecalho { get; set; }
        public IEnumerable<FrequenciaMensalDreDto> FrequenciaMensalDre { get; set; }
    }
}
