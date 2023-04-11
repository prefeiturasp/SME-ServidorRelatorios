using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RespostaSondagemAnaliticaNumeroIadDto : RelatorioSondagemAnaliticoDto
    {
        public List<RespostaSondagemAnaliticaDto> Respostas { get; set; }
    }
}
