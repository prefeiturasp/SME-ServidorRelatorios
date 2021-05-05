using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioRegistroIndividualAlunoDto
    {
        public RelatorioRegistroIndividualAlunoDto()
        {
            Registros = new List<RelatorioRegistroIndividualDetalhamentoDto>();
        }
        public string Nome { get; set; }
        public List<RelatorioRegistroIndividualDetalhamentoDto> Registros { get; set; }
    }
}
