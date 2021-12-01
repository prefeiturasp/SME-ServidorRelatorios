using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioNotificacoesUsuarioDto
    {
        public RelatorioNotificacoesUsuarioDto()
        {
            Notificacoes = new List<NotificacaoRelatorioDto>();
        }

        public string Nome { get; set; }
        public IEnumerable<NotificacaoRelatorioDto> Notificacoes { get; set; }
    }
}
