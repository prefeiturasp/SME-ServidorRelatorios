using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class AdesaoAeUeRetornoDto
    {
        public AdesaoAeUeRetornoDto()
        {
            Modalidades = new List<AdesaoAEModalidadeDto>();
        }
        public AdesaoAEValoresDto Valores { get; set; }

        public List<AdesaoAEModalidadeDto> Modalidades { get; set; }
        
        public bool MostraColunaSituacao { get; set; }

    }
}