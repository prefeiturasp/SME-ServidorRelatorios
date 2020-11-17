using MediatR;
using SME.SR.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Application
{
    public class ObterUePorCodigosQuery : IRequest<IEnumerable<Ue>>
    {
        public ObterUePorCodigosQuery(string[] ueCodigos)
        {
            UeCodigos = ueCodigos;
        }

        public string[] UeCodigos { get; set; }
    }
}
