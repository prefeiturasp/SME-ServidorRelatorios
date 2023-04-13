using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class CabecalhoSondagemAnaliticaDto
    {
        public string Descricao { get; set; }
        public int Ordem { get; set; }
        public List<SubCabecalhoSondagemAnaliticaDto> SubCabecalhos { get; set; }

        public CabecalhoSondagemAnaliticaDto()
        {
            SubCabecalhos = new List<SubCabecalhoSondagemAnaliticaDto>();
        }
    }
}
