using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioFechamentoPendenciasComponenteDto
    {
        public RelatorioFechamentoPendenciasComponenteDto()
        {
            Pendencias = new List<RelatorioFechamentoPendenciasPendenciaDto>();
        }
        public string NomeComponente { get; set; }
        public string CodigoComponente { get; set; }
        public List<RelatorioFechamentoPendenciasPendenciaDto> Pendencias { get; set; }

    }
}
