using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioAcompanhamentoFechamentoConselhoClasseConsolidadoDto
    {
        public RelatorioAcompanhamentoFechamentoConselhoClasseConsolidadoDto(string nomeTurma)
        {
            NomeTurma = nomeTurma;
            FechamentoConsolidado = new RelatorioAcompanhamentoFechamentoConsolidadoDto();
            ConselhoDeClasseConsolidado = new RelatorioAcompanhamentoConselhoClasseConsolidadoDto();
        }

        public string NomeTurma { get; set; }
        public RelatorioAcompanhamentoFechamentoConsolidadoDto FechamentoConsolidado { get; set; }
        public RelatorioAcompanhamentoConselhoClasseConsolidadoDto ConselhoDeClasseConsolidado { get; set; }
    }
}
