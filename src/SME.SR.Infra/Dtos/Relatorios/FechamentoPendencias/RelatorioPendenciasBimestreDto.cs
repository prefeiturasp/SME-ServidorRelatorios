using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioPendenciasBimestreDto
    {
        public RelatorioPendenciasBimestreDto()
        {
            Componentes = new List<RelatorioPendenciasComponenteDto>();
        }
        public string NomeBimestre { get; set; }
        public string NomeModalidade { get; set; }
        public string SemestreTurma { get; set; }

        public List<RelatorioPendenciasComponenteDto> Componentes { get; set; }

    }
}
