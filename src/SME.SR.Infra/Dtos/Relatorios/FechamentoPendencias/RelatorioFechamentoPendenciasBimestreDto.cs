using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioFechamentoPendenciasBimestreDto
    {
        public RelatorioFechamentoPendenciasBimestreDto()
        {
            Componentes = new List<RelatorioFechamentoPendenciasComponenteDto>();
        }
        public string Nome { get; set; }

        public List<RelatorioFechamentoPendenciasComponenteDto> Componentes { get; set; }

    }
}
