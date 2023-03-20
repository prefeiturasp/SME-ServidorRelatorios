using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioSondagemAnaliticoCapacidadeDeLeituraDto : RelatorioSondagemAnaliticoPorDreDto
    {
        public List<RespostaSondagemAnaliticoCapacidadeDeLeituraDto> Respostas { get; set; }
    }
}
