using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterRelatorioAnaliticoSondagemExcelQuery : IRequest<IEnumerable<RelatorioSondagemAnaliticoExcelDto>>
    {
        public IEnumerable<RelatorioSondagemAnaliticoPorDreDto> RelatorioAnalitico { get; set; }
        public TipoSondagem TipoSondagem { get; set; }

        public ObterRelatorioAnaliticoSondagemExcelQuery(IEnumerable<RelatorioSondagemAnaliticoPorDreDto> relatorioSondagemAnaliticoPorDreDtos, TipoSondagem tipoSondagem)
        {
            RelatorioAnalitico = relatorioSondagemAnaliticoPorDreDtos;
            TipoSondagem = tipoSondagem;
        }
    }
}
