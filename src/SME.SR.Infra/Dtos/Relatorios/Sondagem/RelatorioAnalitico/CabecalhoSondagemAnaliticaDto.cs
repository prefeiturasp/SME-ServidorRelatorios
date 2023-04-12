using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class CabecalhoSondagemAnaliticaDto
    {
        public string Descricao { get; set; }
        public List<SubCabecalhoSondagemAnaliticaDto> SubCabecalhos { get; set; }
    }
}
