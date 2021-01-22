using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class UsuarioNotificacaoDto
    {
        public UsuarioNotificacaoDto()
        {
            Notificacoes = new List<NotificacaoDto>();
        }

        public string Nome { get; set; }

        public List<NotificacaoDto> Notificacoes { get; set; }
    }
}
