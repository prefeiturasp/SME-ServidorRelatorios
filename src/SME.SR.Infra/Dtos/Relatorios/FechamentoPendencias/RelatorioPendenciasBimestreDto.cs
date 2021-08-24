using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioPendenciasBimestreDto
    {
        public RelatorioPendenciasBimestreDto()
        {
            Componentes = new List<RelatorioPendenciasComponenteDto>();
        }
        public string Nome { get; set; }

        public List<RelatorioPendenciasComponenteDto> Componentes { get; set; }

    }
}
