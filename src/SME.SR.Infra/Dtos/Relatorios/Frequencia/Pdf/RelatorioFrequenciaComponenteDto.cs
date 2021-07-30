using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioFrequenciaComponenteDto
    {
        public RelatorioFrequenciaComponenteDto()
        {
            Alunos = new List<RelatorioFrequenciaAlunoDto>();
        }

        public string NomeComponente { get; set; }
        public string CodigoComponente { get; set; }
        public List<RelatorioFrequenciaAlunoDto> Alunos { get; set; }
    }
}
