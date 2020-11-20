using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class UeNotificacaoDto
    {
        public UeNotificacaoDto()
        {
            Usuarios = new List<UsuarioNotificacaoDto>();
        }

        public string Nome { get; set; }
        public List<UsuarioNotificacaoDto> Usuarios { get; set; }

    }
}
