using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class FrequenciaMensalMesDto
    {
        public string NomeMes { get; set; }
        public IEnumerable<FrequenciaMensalAlunoDto> FrequenciaMensalAluno { get; set; }
    }
}