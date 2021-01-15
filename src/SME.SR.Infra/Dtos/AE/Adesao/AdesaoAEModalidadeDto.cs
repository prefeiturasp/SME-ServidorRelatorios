using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class AdesaoAEModalidadeDto
    {
        public AdesaoAEModalidadeDto()
        {
            Turmas = new List<AdesaoAETurmaDto>();
        }
        public AdesaoAEValoresDto Valores { get; set; }
        public List<AdesaoAETurmaDto> Turmas { get; set; }
    }
}