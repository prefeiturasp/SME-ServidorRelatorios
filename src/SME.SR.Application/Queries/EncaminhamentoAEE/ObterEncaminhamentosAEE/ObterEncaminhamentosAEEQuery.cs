using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterEncaminhamentosAEEQuery : IRequest<IEnumerable<EncaminhamentoAeeDto>>
    {
        public ObterEncaminhamentosAEEQuery(FiltroRelatorioEncaminhamentoAeeDto filtro)
        {
            Filtro = filtro;
        }

        public FiltroRelatorioEncaminhamentoAeeDto Filtro { get; }
    }
}
