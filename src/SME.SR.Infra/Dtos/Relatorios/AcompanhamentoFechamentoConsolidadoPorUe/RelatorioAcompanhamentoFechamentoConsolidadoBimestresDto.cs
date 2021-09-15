using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioAcompanhamentoFechamentoConsolidadoBimestresDto
    {
        public RelatorioAcompanhamentoFechamentoConsolidadoBimestresDto(string bimestre, string tumaCodigo)
        {
            Bimestre = bimestre;
            FechamentoConselhoClasseConsolidado = new List<RelatorioAcompanhamentoFechamentoConselhoClasseConsolidadoDto>();
            TumaCodigo = tumaCodigo;
        }

        public string Bimestre { get; set; }
        public string TumaCodigo { get; set; }

        public List<RelatorioAcompanhamentoFechamentoConselhoClasseConsolidadoDto> FechamentoConselhoClasseConsolidado { get; set; }
    }
}
