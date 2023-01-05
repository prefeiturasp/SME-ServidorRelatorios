using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class EncaminhamentoAEEQuery : IRequest<IEnumerable<EncaminhamentoAeeDto>>
    {
        public EncaminhamentoAEEQuery(FiltroRelatorioEncaminhamentoAeeDto filtro)
        {
            Filtro = filtro;
        }

        public FiltroRelatorioEncaminhamentoAeeDto Filtro { get; }
    }
}
