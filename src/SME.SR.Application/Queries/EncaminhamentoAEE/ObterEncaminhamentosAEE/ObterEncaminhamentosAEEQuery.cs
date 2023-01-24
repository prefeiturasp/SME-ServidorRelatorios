using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterEncaminhamentosAEEQuery : IRequest<IEnumerable<EncaminhamentoAeeDto>>
    {
        public ObterEncaminhamentosAEEQuery(FiltroRelatorioEncaminhamentosAeeDto filtro)
        {
            Filtro = filtro;
        }

        public FiltroRelatorioEncaminhamentosAeeDto Filtro { get; }
    }
}
