using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioSondagemAnaliticoLeituraDto : RelatorioSondagemAnaliticoPorDreDto
    {
        public List<RespostaSondagemAnaliticoLeituraDto> Respostas { get; set; }

        protected override TipoSondagem TipoDaSondagem => TipoSondagem.LP_Leitura;
    }
}
