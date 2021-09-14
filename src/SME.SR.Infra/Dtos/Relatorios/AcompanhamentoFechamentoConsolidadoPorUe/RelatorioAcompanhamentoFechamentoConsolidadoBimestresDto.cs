using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioAcompanhamentoFechamentoConsolidadoBimestresDto
    {
        public RelatorioAcompanhamentoFechamentoConsolidadoBimestresDto(string bimestre, string nomeTurma)
        {
            Bimestre = bimestre;
            FechamentoConsolidado = new List<RelatorioAcompanhamentoFechamentoConsolidadoDto>();
            ConselhoDeClasseConsolidado = new List<RelatorioAcompanhamentoConselhoClasseConsolidadoDto>();
            NomeTurma = nomeTurma;
        }

        public string Bimestre { get; set; }
        public string NomeTurma { get; set; }

        public List<RelatorioAcompanhamentoFechamentoConsolidadoDto> FechamentoConsolidado { get; set; }
        public List<RelatorioAcompanhamentoConselhoClasseConsolidadoDto> ConselhoDeClasseConsolidado { get; set; }
    }
}
