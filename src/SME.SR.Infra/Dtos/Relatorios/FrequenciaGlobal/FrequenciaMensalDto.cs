using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class FrequenciaMensalDto
    {
        public FrequenciaMensalDto()
        {
            Cabecalho = new FrequenciaMensalCabecalhoDto();
            Dres = new List<FrequenciaMensalDreDto>();
        }
        public FrequenciaMensalCabecalhoDto Cabecalho { get; set; }
        public List<FrequenciaMensalDreDto> Dres { get; set; }
    }
}
