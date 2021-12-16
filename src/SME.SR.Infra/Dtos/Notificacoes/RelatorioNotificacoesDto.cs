using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioNotificacoesDto
    {
        public RelatorioNotificacoesDto()
        {
            Usuarios = new List<RelatorioNotificacoesUsuarioDto>();
        }

        public RelatorioNotificacoesCabecalhoDto Cabecalho { get; set; }
        public IEnumerable<RelatorioNotificacoesUsuarioDto> Usuarios { get; set; }
    }
}
