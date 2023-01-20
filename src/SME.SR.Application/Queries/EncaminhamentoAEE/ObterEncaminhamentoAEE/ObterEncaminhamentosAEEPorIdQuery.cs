using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterEncaminhamentosAEEPorIdQuery : IRequest<IEnumerable<EncaminhamentoAeeDto>>
    {
        public ObterEncaminhamentosAEEPorIdQuery(FiltroRelatorioEncaminhamentoAeeDetalhadoDto filtro)
        {
            Filtro = filtro;
        }

        public FiltroRelatorioEncaminhamentoAeeDetalhadoDto Filtro { get; }
    }
}
