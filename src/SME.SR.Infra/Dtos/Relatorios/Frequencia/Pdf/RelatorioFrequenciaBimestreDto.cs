using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioFrequenciaBimestreDto
    {
        public RelatorioFrequenciaBimestreDto()
        {
            Componentes = new List<RelatorioFrequenciaComponenteDto>();
        }

        public string NomeBimestre { get; set; }
        public string Numero { get; set; }
        public List<RelatorioFrequenciaComponenteDto> Componentes { get; set; }
    }
}
