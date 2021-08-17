using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioPendenciasTurmaDto
    {
        public RelatorioPendenciasTurmaDto()
        {
            Bimestres = new List<RelatorioPendenciasBimestreDto>();
        }
        public string Nome { get; set; }
        public List<RelatorioPendenciasBimestreDto> Bimestres { get; set; }

    }
}
