using MediatR;
using SME.SR.Data.Models;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterParametrosMediaFrequenciaQuery : IRequest<IEnumerable<MediaFrequencia>>
    {
    }
}
