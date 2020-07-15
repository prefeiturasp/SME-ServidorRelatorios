using MediatR;
using System;

namespace SME.SR.Application
{
    public class GerarCodigoCorrelacaoSGPCommand : IRequest<Guid>
    {
        public GerarCodigoCorrelacaoSGPCommand(Guid codigoCorrelacaoParaCopiar)
        {
            CodigoCorrelacaoParaCopiar = codigoCorrelacaoParaCopiar;
        }
        public Guid CodigoCorrelacaoParaCopiar { get; set; }
    }
}
