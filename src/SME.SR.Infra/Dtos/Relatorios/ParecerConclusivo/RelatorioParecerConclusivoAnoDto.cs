using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioParecerConclusivoAnoDto
    {
        public RelatorioParecerConclusivoAnoDto()
        {
            PareceresConclusivos = new List<RelatorioParecerConclusivoAlunoDto>();
        }
        public string Nome { get; set; }

        public List<RelatorioParecerConclusivoAlunoDto> PareceresConclusivos { get; set; }
    }
}