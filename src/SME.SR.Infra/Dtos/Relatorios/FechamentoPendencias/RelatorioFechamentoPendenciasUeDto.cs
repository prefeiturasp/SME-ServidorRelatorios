using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioFechamentoPendenciasUeDto
    {
        public RelatorioFechamentoPendenciasUeDto()
        {
            Turmas = new List<RelatorioFechamentoPendenciasTurmaDto>();
        }
        public string Codigo { get; set; }
        public string Nome { get; set; }
        public List<RelatorioFechamentoPendenciasTurmaDto> Turmas { get; set; }
    }
}
