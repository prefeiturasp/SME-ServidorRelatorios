using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class FrequenciaMensalUeDto
    {
        public string CodigoUe { get; set; }
        public string NomeUe { get; set; }
        public IEnumerable<FrequenciaMensalTurmaDto> FrequenciaMensalTurma { get; set; }
    }
}