using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioAcompanhamentoFechamentoConsolidadoUesDto
    {
        public RelatorioAcompanhamentoFechamentoConsolidadoUesDto()
        {
            Turmas = new List<RelatorioAcompanhamentoFechamentoConsolidadoTurmasDto>();
        }

        public string NomeUe { get; set; }
        public List<RelatorioAcompanhamentoFechamentoConsolidadoTurmasDto> Turmas{ get; set; }
    }
}
