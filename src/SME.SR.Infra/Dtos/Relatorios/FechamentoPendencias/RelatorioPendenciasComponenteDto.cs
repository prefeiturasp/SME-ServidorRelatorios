using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioPendenciasComponenteDto
    {
        public RelatorioPendenciasComponenteDto()
        {
            Pendencias = new List<RelatorioPendenciasPendenciaDto>();
        }
        public string NomeComponente { get; set; }
        public string CodigoComponente { get; set; }
        public List<RelatorioPendenciasPendenciaDto> Pendencias { get; set; }

    }
}
