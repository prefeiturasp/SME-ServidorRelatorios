using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioSondagemAnaliticoProducaoDeTextoDto : RelatorioSondagemAnaliticoPorDreDto
    {
        public RelatorioSondagemAnaliticoProducaoDeTextoDto()
        {
            Respostas = new List<RespostaSondagemAnaliticoProducaoDeTextoDto>();
        }
        public List<RespostaSondagemAnaliticoProducaoDeTextoDto> Respostas { get; set; }

        protected override TipoSondagem TipoDaSondagem => TipoSondagem.LP_ProducaoTexto;
    }
}
