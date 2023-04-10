using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RespostaSondagemAnaliticoCampoAditivoMultiplicativoDto : RelatorioSondagemAnaliticoDto
    {
        public RespostaSondagemAnaliticoCampoAditivoMultiplicativoDto()
        {
            Ordens = new List<RespostaOrdemMatematicaDto>();
        }

        public List<RespostaOrdemMatematicaDto> Ordens { get; set; }
    }
}
