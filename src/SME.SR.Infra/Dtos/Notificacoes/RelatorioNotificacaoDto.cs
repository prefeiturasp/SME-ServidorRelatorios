using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class RelatorioNotificacaoDto
    {
        public RelatorioNotificacaoDto()
        {
            DREs = new List<DreNotificacaoDto>();
        }

        public string CabecalhoDRE { get; set; }
        public string CabecalhoUE { get; set; }
        public string CabecalhoUsuario { get; set; }
        public string CabecalhoUsuarioRF { get; set; }

        public DateTime DataRelatorio { get => DateTime.Now; }

        public List<DreNotificacaoDto> DREs { get; set; }
    }
}
