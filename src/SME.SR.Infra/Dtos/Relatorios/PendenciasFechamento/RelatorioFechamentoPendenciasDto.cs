using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioFechamentoPendenciasDto
    {
        public RelatorioFechamentoPendenciaCabecalhoDto Cabecalho { get; set; }
        public bool MostraDetalhamento { get; set; }
        public List<RelatorioFechamentoPendenciaBimestreDto> Bimestres { get; set; }

        public RelatorioFechamentoPendenciasDto()
        {
            Bimestres = new List<RelatorioFechamentoPendenciaBimestreDto>();
        }
    }
}
