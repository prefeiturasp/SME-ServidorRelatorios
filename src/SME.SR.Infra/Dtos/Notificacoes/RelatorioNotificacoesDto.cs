using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioNotificacoesDto
    {
        public RelatorioNotificacoesDto()
        {
            Usuarios = new List<RelatorioNotificacoesUsuario>();
        }

        public RelatorioNotificacoesCabecalhoDto Cabecalho { get; set; }
        public IEnumerable<RelatorioNotificacoesUsuario> Usuarios { get; set; }
    }
}
