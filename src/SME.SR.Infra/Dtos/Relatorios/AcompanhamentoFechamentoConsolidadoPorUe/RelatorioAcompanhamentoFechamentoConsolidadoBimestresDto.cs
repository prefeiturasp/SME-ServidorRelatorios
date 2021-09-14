using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioAcompanhamentoFechamentoConsolidadoBimestresDto
    {
        public RelatorioAcompanhamentoFechamentoConsolidadoBimestresDto(string bimestre)
        {
            Bimestre = bimestre;
            FechamentoConselhoClasseConsolidado = new List<RelatorioAcompanhamentoFechamentoConselhoClasseConsolidadoDto>();
        }

        public string Bimestre { get; set; }
        public List<RelatorioAcompanhamentoFechamentoConselhoClasseConsolidadoDto> FechamentoConselhoClasseConsolidado { get; set; }
    }
}
