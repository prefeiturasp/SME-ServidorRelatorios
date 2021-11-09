using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioNotificacoesUsuario
    {
        public RelatorioNotificacoesUsuario()
        {
            Notificacoes = new List<NotificacaoDto>();
        }

        public string Nome { get; set; }
        public IEnumerable<NotificacaoDto> Notificacoes { get; set; }
    }
}
