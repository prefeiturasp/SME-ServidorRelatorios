using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioFechamentoPendenciaBimestreDto
    {
        public string Bimestre { get; set; }

        public List<RelatorioFechamentoPendenciaDto> Pendencias { get; set; }

        public RelatorioFechamentoPendenciaBimestreDto()
        {
            Pendencias = new List<RelatorioFechamentoPendenciaDto>();
        }

    }
}