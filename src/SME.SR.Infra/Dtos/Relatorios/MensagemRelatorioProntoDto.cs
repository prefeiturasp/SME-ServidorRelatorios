using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class MensagemRelatorioProntoDto
    {
        public MensagemRelatorioProntoDto()
        {
        }

        public MensagemRelatorioProntoDto(string mensagemUsuario)
        {
            MensagemUsuario = mensagemUsuario;
        }

        public string MensagemUsuario { get; set; }
    }
}
