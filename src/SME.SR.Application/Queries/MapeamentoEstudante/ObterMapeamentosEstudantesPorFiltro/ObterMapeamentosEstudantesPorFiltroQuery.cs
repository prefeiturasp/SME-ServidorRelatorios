using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterMapeamentosEstudantesPorFiltroQuery : IRequest<IEnumerable<MapeamentoEstudanteUltimoBimestreDto>>
    {
        public ObterMapeamentosEstudantesPorFiltroQuery(FiltroRelatorioMapeamentoEstudantesDto filtro)
        {
            Filtro = filtro;
        }

        public FiltroRelatorioMapeamentoEstudantesDto Filtro { get; }
    }
}
