using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioFaltaFrequenciaComponenteDto
    {
        public RelatorioFaltaFrequenciaComponenteDto()
        {
            Alunos = new List<RelatorioFaltaFrequenciaAlunoDto>();
        }
        public string Nome { get; set; }
        public List<RelatorioFaltaFrequenciaAlunoDto> Alunos { get; set; }
    }
}
