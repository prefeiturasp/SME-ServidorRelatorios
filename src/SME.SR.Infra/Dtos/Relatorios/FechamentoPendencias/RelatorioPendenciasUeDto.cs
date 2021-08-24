using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioPendenciasUeDto
    {
        public RelatorioPendenciasUeDto()
        {
            Turmas = new List<RelatorioPendenciasTurmaDto>();
        }
        public string Codigo { get; set; }
        public string Nome { get; set; }
        public List<RelatorioPendenciasTurmaDto> Turmas { get; set; }
    }
}
