using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class FrequenciaMensalTurmaDto
    {
        public FrequenciaMensalTurmaDto()
        {
            Meses = new List<FrequenciaMensalMesDto>();
        }
        public string NomeTurma { get; set; }
        public string CodigoTurma { get; set; }
        public string CodigoUe { get; set; }
        public List<FrequenciaMensalMesDto> Meses { get; set; }
    }
}