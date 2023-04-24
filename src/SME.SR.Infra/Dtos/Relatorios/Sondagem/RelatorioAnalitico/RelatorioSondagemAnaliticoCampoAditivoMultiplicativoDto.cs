using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioSondagemAnaliticoCampoAditivoMultiplicativoDto : RelatorioSondagemAnaliticoPorDreDto
    {
        private TipoSondagem _tipoSondagem;

        public RelatorioSondagemAnaliticoCampoAditivoMultiplicativoDto(TipoSondagem tipoSondagem)
        {
            _tipoSondagem = tipoSondagem;
            Respostas = new List<RespostaSondagemAnaliticoCampoAditivoMultiplicativoDto>();
        }

        public List<RespostaSondagemAnaliticoCampoAditivoMultiplicativoDto> Respostas { get; set; }
        protected override TipoSondagem TipoDaSondagem => _tipoSondagem;
    }
}
