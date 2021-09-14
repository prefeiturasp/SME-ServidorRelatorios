using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioAcompanhamentoFechamentoConsolidadoUesDto
    {
        public RelatorioAcompanhamentoFechamentoConsolidadoUesDto(string nomeUe)
        {
            Turmas = new List<RelatorioAcompanhamentoFechamentoConsolidadoTurmasDto>();
            NomeUe = nomeUe;
        }

        public string NomeUe { get; set; }
        public List<RelatorioAcompanhamentoFechamentoConsolidadoTurmasDto> Turmas{ get; set; }
    }
}
