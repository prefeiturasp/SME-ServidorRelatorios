using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioFrequenciaDto
    {
        public RelatorioFrequenciaDto()
        {
            Dres = new List<RelatorioFrequenciaDreDto>();
            Cabecalho = new RelatorioFrequenciaCabecalhoDto();
        }

        public string UltimoAluno { get; set; }
        public RelatorioFrequenciaCabecalhoDto Cabecalho { get; set; }
        public List<RelatorioFrequenciaDreDto> Dres { get; set; }
    }
}
