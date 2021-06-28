using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioAcompanhamentoFechamentoTurmaDto
    {
        public RelatorioAcompanhamentoFechamentoTurmaDto()
        {
            FechamentosComponente = new List<RelatorioAcompanhamentoFechamentoComponenteDto>();
            ConselhosClasse = new List<RelatorioAcompanhamentoFechamentoConselhoDto>();
        }

        public string TurmaDescricao { get; set; }

        public List<RelatorioAcompanhamentoFechamentoComponenteDto> FechamentosComponente { get; set; }

        public List<RelatorioAcompanhamentoFechamentoConselhoDto> ConselhosClasse { get; set; }
    }
}
