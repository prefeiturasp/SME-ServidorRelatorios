using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioSondagemAnaliticoEscritaDto : RelatorioSondagemAnaliticoPorDreDto
    {
        public List<RespostaSondagemAnaliticoEscritaDto> Respostas { get; set; }
    }
}
