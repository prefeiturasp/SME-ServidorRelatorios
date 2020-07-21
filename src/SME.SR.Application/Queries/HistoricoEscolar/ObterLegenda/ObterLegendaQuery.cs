using System.Collections.Generic;
using MediatR;
using SME.SR.Infra;

namespace SME.SR.Application
{
    public class ObterLegendaQuery : IRequest<IEnumerable<ConceitoDto>>
    {

    }
}
