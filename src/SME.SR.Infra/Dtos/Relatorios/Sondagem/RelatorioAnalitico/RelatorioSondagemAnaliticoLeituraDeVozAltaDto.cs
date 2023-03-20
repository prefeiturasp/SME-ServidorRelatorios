using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioSondagemAnaliticoLeituraDeVozAltaDto : RelatorioSondagemAnaliticoPorDreDto
    {
        public List<RespostaSondagemAnaliticoLeituraDeVozAltaDto> Respostas { get; set; }
    }
}
