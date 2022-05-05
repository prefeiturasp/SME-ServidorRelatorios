using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class FrequenciaMensalUeDto
    {
        public FrequenciaMensalUeDto()
        {
            Turmas = new List<FrequenciaMensalTurmaDto>();
        }
        public string CodigoUe { get; set; }
        public string CodigoDre { get; set; }
        public string NomeUe { get; set; }
        public List<FrequenciaMensalTurmaDto> Turmas { get; set; }
    }
}