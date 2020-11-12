using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class RelatorioNotificacaoDto
    {
        public RelatorioNotificacaoDto()
        {
            Filtro = new FiltroNotificacaoDto();
            DREs = new List<DreNotificacaoDto>();
        }

        public FiltroNotificacaoDto Filtro { get; set; }

        public List<DreNotificacaoDto> DREs { get; set; }
    }
}
