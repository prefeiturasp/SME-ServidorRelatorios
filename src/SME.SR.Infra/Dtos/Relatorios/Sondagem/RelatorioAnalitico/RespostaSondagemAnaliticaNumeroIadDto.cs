using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RespostaSondagemAnaliticaNumeroIadDto : RelatorioSondagemAnaliticoDto
    {
        public RespostaSondagemAnaliticaNumeroIadDto()
        {
            Respostas = new List<RespostaSondagemAnaliticaDto>();
        }
        public List<RespostaSondagemAnaliticaDto> Respostas { get; set; }
    }
}
