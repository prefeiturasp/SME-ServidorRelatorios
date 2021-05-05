using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioRegistroIndividualDto
    {
        public RelatorioRegistroIndividualDto()
        {
            Alunos = new List<RelatorioRegistroIndividualAlunoDto>();
        }
        public RelatorioRegistroIndividualCabecalhoDto Cabecalho { get; set; }
        public List<RelatorioRegistroIndividualAlunoDto> Alunos { get; set; }
    }
}
