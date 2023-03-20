using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterRelatorioAnaliticoSondagemQuery : IRequest<IEnumerable<RelatorioSondagemAnaliticoPorDreDto>>
    {
        public FiltroRelatorioAnaliticoSondagemDto Filtro { get; set; }

        public ObterRelatorioAnaliticoSondagemQuery(FiltroRelatorioAnaliticoSondagemDto filtro)
        {
            Filtro = filtro;
        }
    }
}
