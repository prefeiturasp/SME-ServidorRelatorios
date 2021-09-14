using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioAcompanhamentoFechamentoConselhoClasseConsolidadoDto
    {
        public RelatorioAcompanhamentoFechamentoConselhoClasseConsolidadoDto(string nomeTurma)
        {
            NomeTurma = nomeTurma;
            FechamentoConsolidado = new List<RelatorioAcompanhamentoFechamentoConsolidadoDto>();
            ConselhoDeClasseConsolidado = new List<RelatorioAcompanhamentoConselhoClasseConsolidadoDto>();
        }

        public string NomeTurma { get; set; }
        public List<RelatorioAcompanhamentoFechamentoConsolidadoDto> FechamentoConsolidado { get; set; }
        public List<RelatorioAcompanhamentoConselhoClasseConsolidadoDto> ConselhoDeClasseConsolidado { get; set; }
    }
}
