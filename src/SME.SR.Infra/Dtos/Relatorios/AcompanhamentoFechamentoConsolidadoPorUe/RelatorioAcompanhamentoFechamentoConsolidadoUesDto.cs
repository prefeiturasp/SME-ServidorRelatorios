using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioAcompanhamentoFechamentoConsolidadoUesDto
    {
        public RelatorioAcompanhamentoFechamentoConsolidadoUesDto(string nomeTurma, string nomeUe, string nomeBimestre)
        {
            FechamentoConselhoClasseConsolidado = new List<RelatorioAcompanhamentoFechamentoConselhoClasseConsolidadoDto>();
            NomeTurma = nomeTurma;
            NomeUe = nomeUe;
            NomeBimestre = nomeBimestre;
        }

        public string NomeTurma { get; set; }
        public string NomeUe { get; set; }
        public string NomeBimestre { get; set; }
        public List<RelatorioAcompanhamentoFechamentoConselhoClasseConsolidadoDto> FechamentoConselhoClasseConsolidado { get; set; }
    }
}
