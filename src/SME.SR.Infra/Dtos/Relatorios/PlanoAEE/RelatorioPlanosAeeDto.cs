using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioPlanosAeeDto
    {
        public RelatorioPlanosAeeDto()
        {
            Cabecalho = new CabecalhoPlanosAeeDto();
            AgrupamentosDreUe = new List<AgrupamentoDreUeDto>();
        }        
        
        public CabecalhoPlanosAeeDto Cabecalho { get; set; }

        public List<AgrupamentoDreUeDto> AgrupamentosDreUe { get; set; } }
}