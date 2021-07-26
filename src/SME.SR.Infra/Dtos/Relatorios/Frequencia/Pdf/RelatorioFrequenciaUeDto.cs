using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioFrequenciaUeDto
    {
        public RelatorioFrequenciaUeDto()
        {
            TurmasAnos = new List<RelatorioFrequenciaTurmaAnoDto>();
        }

        public string CodigoUe { get; set; }
        public string NomeUe { get; set; }
        public TipoEscola TipoUe { get; set; }
        public List<RelatorioFrequenciaTurmaAnoDto> TurmasAnos { get; set; }
    }
}
