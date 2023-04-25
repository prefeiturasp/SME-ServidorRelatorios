using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioSondagemAnaliticoLeituraDeVozAltaDto : RelatorioSondagemAnaliticoPorDreDto
    {
        public RelatorioSondagemAnaliticoLeituraDeVozAltaDto()
        {
            Respostas = new List<RespostaSondagemAnaliticoLeituraDeVozAltaDto>();
        }
        public List<RespostaSondagemAnaliticoLeituraDeVozAltaDto> Respostas { get; set; }

        protected override TipoSondagem TipoDaSondagem => TipoSondagem.LP_LeituraVozAlta;
    }
}
