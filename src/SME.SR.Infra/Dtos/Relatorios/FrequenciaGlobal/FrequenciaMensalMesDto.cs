using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class FrequenciaMensalMesDto
    {
        public FrequenciaMensalMesDto()
        {
            Alunos = new List<FrequenciaMensalAlunoDto>();
        }
        public string NomeMes { get; set; }
        public string TurmaCodigo { get; set; }
        public int ValorMes { get; set; }
        public List<FrequenciaMensalAlunoDto> Alunos { get; set; }
    }
}