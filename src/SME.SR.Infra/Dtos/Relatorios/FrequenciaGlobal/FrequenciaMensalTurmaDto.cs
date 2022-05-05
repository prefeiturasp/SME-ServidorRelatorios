using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class FrequenciaMensalTurmaDto
    {
        public string CodigoTurma { get; set; }
        public string NomeTurma { get; set; }
        public IEnumerable<FrequenciaMensalMesDto> FrequenciaMensalMes { get; set; }
    }
}