using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class AgrupamentoEncaminhamentoNAAPADreUeDto
    {
        public AgrupamentoEncaminhamentoNAAPADreUeDto()
        {
            Detalhes = new List<DetalheEncaminhamentoNAAPADto>();
        }

        public long DreId { get; set; }
        public string DreNome { get; set; }
        public string UeNome { get; set; }
        public string UeOrdenacao { get; set; }
        public bool MostrarAgrupamento { get; set; }
        public List<DetalheEncaminhamentoNAAPADto> Detalhes { get; set; }
    }
}
