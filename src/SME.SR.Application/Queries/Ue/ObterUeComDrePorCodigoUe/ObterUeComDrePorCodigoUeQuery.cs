using System;
using MediatR;
using SME.SR.Data;

namespace SME.SR.Application
{
    public class ObterUeComDrePorCodigoUeQuery : IRequest<Ue>
    {
        public ObterUeComDrePorCodigoUeQuery(string ueCodigo)
        {
            UeCodigo = ueCodigo ?? throw new ArgumentNullException(nameof(ueCodigo));
        }

        public string UeCodigo { get; set; }
    }
}