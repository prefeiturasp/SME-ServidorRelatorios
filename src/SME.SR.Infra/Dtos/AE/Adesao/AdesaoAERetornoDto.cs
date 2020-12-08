using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class AdesaoAERetornoDto
    {
        
        public string DRENome { get; set; }
        public string UeNome { get; set; }
        public string UsuarioNome { get; set; }
        public string UsuarioRF { get; set; }
        public string Data { get; set; }
        public bool MostraSME { get; set; }
        public bool MostraDRE { get; set; }
        public bool MostraUe { get; set; }
        public AdesaoAESmeDto SME { get; set; }
        public AdesaoAEDreDto DRE { get; set; }
        public AdesaoAeUeRetornoDto UE { get; set; }       
    }
}
