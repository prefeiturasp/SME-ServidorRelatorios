using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioCompensacaoAusenciaTurmaDto
    {
        public RelatorioCompensacaoAusenciaTurmaDto()
        {
            Bimestres = new List<RelatorioCompensacaoAusenciaBimestreDto>();
        }

        public string Nome { get; set; }
        public List<RelatorioCompensacaoAusenciaBimestreDto> Bimestres { get; set; }
    }
}

