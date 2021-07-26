using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioFrequenciaTurmaAnoDto
    {
        public RelatorioFrequenciaTurmaAnoDto()
        {
            Bimestres = new List<RelatorioFrequenciaBimestreDto>();
        }

        public string Nome { get; set; }
        public List<RelatorioFrequenciaBimestreDto> Bimestres { get; set; }
    }
}
