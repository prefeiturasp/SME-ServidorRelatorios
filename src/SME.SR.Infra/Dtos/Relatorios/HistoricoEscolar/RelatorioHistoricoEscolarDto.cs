using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioHistoricoEscolarDto
    {
        public int TotalPagina { get; set; }

        public IEnumerable<RelatorioPaginadoHistoricoEscolarDto> RelatorioPaginadados { get; set; }
    }
}
