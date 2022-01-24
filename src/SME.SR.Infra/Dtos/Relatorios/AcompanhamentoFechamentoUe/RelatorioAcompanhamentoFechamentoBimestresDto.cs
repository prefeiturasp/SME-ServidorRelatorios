using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioAcompanhamentoFechamentoBimestresDto
    {
        public RelatorioAcompanhamentoFechamentoBimestresDto()
        {
            Turmas = new List<RelatorioAcompanhamentoFechamentoConselhoClasseDto>();            
        }

        public string Bimestre { get; set; }
        public string TumaCodigo { get; set; }
        public List<RelatorioAcompanhamentoFechamentoConselhoClasseDto> Turmas { get; set; }
    }
}
