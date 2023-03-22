using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioSondagemAnaliticoProducaoDeTextoDto : RelatorioSondagemAnaliticoPorDreDto
    {
        public List<RespostaSondagemAnaliticoProducaoDeTextoDto> Respostas { get; set; }

        protected override TipoSondagem TipoDaSondagem => TipoSondagem.LP_ProducaoTexto;
    }
}
