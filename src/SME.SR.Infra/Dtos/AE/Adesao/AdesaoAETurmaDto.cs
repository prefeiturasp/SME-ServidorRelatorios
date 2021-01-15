using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class AdesaoAETurmaDto
    {
        public AdesaoAETurmaDto()
        {
            Alunos = new List<AdesaoAEUeAlunoDto>();
        }
        public AdesaoAEValoresDto Valores { get; set; }
        public List<AdesaoAEUeAlunoDto> Alunos { get; set; }
    }
}