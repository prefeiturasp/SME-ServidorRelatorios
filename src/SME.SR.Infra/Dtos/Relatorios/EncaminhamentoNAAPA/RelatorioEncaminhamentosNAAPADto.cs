using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioEncaminhamentosNAAPADto
    {
        public RelatorioEncaminhamentosNAAPADto()
        {
            EncaminhamentosDreUe = new List<AgrupamentoEncaminhamentoNAAPADreUeDto>();
        }
        public string DreNome { get; set; }
        public string UeNome { get; set; }
        public string UsuarioNome { get; set; }
        public List<AgrupamentoEncaminhamentoNAAPADreUeDto> EncaminhamentosDreUe { get; set; }
    }
}
