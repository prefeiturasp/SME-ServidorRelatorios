using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioCompensacaoAusenciaUeDto
    {
        public RelatorioCompensacaoAusenciaUeDto()
        {
            Turmas = new List<RelatorioCompensacaoAusenciaTurmaDto>();
        }
        public string Codigo { get; set; }
        public string Nome { get; set; }
        public List<RelatorioCompensacaoAusenciaTurmaDto> Turmas { get; set; }
    }
}

