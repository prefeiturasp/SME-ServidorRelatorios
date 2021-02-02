using MediatR;
using SME.SR.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Application
{
    public class ObterPerfisPrioritariosQuery : IRequest<IEnumerable<PrioridadePerfil>>
    {
    }
}
