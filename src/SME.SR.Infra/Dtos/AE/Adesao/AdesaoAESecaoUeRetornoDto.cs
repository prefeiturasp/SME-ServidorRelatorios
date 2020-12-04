using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class AdesaoAESecaoUeRetornoDto
    {
        public AdesaoAESecaoUeRetornoDto()
        {
            Alunos = new List<AdesaoAESecaoUeAlunoRetornoDto>();
        }
        public AdesaoAESecaoLinhaRetornoDto Valores { get; set; }
        public List<AdesaoAESecaoUeAlunoRetornoDto> Alunos { get; set; }
    }
}