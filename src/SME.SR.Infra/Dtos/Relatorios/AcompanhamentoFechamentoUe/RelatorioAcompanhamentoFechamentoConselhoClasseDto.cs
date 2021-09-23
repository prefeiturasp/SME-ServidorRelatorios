using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioAcompanhamentoFechamentoConselhoClasseDto
    {
        public RelatorioAcompanhamentoFechamentoConselhoClasseDto()
        {            
            FechamentoConsolidado = new RelatorioConsolidadoFechamento();
            ConselhoDeClasseConsolidado = new RelatorioConsolidadoConselhoClasse();
        }

        public string NomeTurma { get; set; }
        public RelatorioConsolidadoFechamento FechamentoConsolidado { get; set; }
        public RelatorioConsolidadoConselhoClasse ConselhoDeClasseConsolidado { get; set; }
    }
}
