using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioAcompanhamentoFechamentoConsolidadoBimestresDto
    {
        public RelatorioAcompanhamentoFechamentoConsolidadoBimestresDto(string bimestre)
        {
            Bimestre = bimestre;
            FechamentoConsolidado = new List<RelatorioAcompanhamentoFechamentoConsolidadoDto>();
            ConselhoDeClasseConsolidado = new List<RelatorioAcompanhamentoConselhoClasseConsolidadoDto>();
        }

        public string Bimestre { get; set; }

        public List<RelatorioAcompanhamentoFechamentoConsolidadoDto> FechamentoConsolidado { get; set; }
        public List<RelatorioAcompanhamentoConselhoClasseConsolidadoDto> ConselhoDeClasseConsolidado { get; set; }
    }
}
