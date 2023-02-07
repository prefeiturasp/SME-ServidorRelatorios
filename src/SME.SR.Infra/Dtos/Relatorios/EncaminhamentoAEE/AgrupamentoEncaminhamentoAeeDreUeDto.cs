using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class AgrupamentoEncaminhamentoAeeDreUeDto
    {
        public AgrupamentoEncaminhamentoAeeDreUeDto()
        {
            Detalhes = new List<DetalheEncaminhamentoAeeDto>();
        }

        public long DreId { get; set; }
        public string DreNome { get; set; }
        public string UeNome { get; set; }
        public string UeOrdenacao { get; set; }
        public List<DetalheEncaminhamentoAeeDto> Detalhes { get; set; }
    }
}
