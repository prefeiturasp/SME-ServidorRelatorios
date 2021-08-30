using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioAcompanhamentoFechamentoBimestreDto
    {
        public RelatorioAcompanhamentoFechamentoBimestreDto(string bimestre)
        {
            Bimestre = bimestre;
            FechamentosComponente = new List<RelatorioAcompanhamentoFechamentoComponenteDto>();
            ConselhosClasse = new List<RelatorioAcompanhamentoFechamentoConselhoDto>();
        }

        public string Bimestre { get; set; }

        public List<RelatorioAcompanhamentoFechamentoComponenteDto> FechamentosComponente { get; set; }

        public List<RelatorioAcompanhamentoFechamentoConselhoDto> ConselhosClasse { get; set; }
    }
}
