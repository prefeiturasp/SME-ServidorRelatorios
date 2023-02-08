using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioEncaminhamentoAeeDto
    {
        public RelatorioEncaminhamentoAeeDto()
        {
            Cabecalho = new CabecalhoEncaminhamentoAeeDto();
            AgrupamentosDreUe = new List<AgrupamentoEncaminhamentoAeeDreUeDto>();
        }

        public CabecalhoEncaminhamentoAeeDto Cabecalho { get; set; }

        public List<AgrupamentoEncaminhamentoAeeDreUeDto> AgrupamentosDreUe { get; set; }
    }
}
