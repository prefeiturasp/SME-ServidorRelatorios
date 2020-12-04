using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class AdesaoAERetornoDto
    {
        public AdesaoAERetornoDto()
        {
            Secoes = new List<AdesaoAESecaoRetornoDto>();
        }
        public string DRENome { get; set; }
        public string UeNome { get; set; }
        public string UsuarioNome { get; set; }
        public string UsuarioRF { get; set; }
        public string Data { get; set; }

        public List<AdesaoAESecaoRetornoDto> Secoes { get; set; }

    }
}
