using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Data
{
    public class UsuarioCoreSSO
    {
        public string Senha { get; set; }
        public TipoCriptografia TipoCriptografia { get; set; }
        public SituacaoUsuario Situacao { get; set; }
    }
}
