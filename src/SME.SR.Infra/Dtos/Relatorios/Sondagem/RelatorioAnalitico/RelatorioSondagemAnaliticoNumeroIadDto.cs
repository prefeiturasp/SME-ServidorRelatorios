using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioSondagemAnaliticoNumeroIadDto : RelatorioSondagemAnaliticoPorDreDto
    {
        private TipoSondagem tipoSondagem;

        public RelatorioSondagemAnaliticoNumeroIadDto(TipoSondagem tipoSondagem)
        {
            this.tipoSondagem = tipoSondagem;
            Respostas = new List<RespostaSondagemAnaliticaNumeroIadDto>();
        }

        public List<CabecalhoSondagemAnaliticaDto> ColunasDoCabecalho { get; set; }

        public List<RespostaSondagemAnaliticaNumeroIadDto> Respostas { get; set; }

        protected override TipoSondagem TipoDaSondagem => tipoSondagem;
    }
}
