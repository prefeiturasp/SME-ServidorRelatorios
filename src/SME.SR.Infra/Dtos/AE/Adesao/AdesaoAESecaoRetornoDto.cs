using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class AdesaoAESecaoRetornoDto
    {
        public AdesaoAESecaoRetornoDto()
        {
            DREs = new List<AdesaoAESecaoLinhaRetornoDto>();
        }
        public bool MostraSME { get; set; }
        public bool MostraDRE { get; set; }
        public AdesaoAESecaoLinhaRetornoDto SME { get; set; }
        public List<AdesaoAESecaoLinhaRetornoDto> DREs { get; set; }
        public AdesaoAESecaoUeRetornoDto UE { get; set; }

    }
}