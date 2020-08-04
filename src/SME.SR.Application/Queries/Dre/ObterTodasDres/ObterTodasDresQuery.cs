using MediatR;
using SME.SR.Data;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterTodasDresQuery : IRequest<IEnumerable<Dre>>
    {
    }
}
