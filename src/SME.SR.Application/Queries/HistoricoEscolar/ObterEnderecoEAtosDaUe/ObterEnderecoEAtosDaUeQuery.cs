using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterEnderecoEAtosDaUeQuery : IRequest<IEnumerable<EnderecoEAtosDaUeDto>>
    {
        public string UeCodigo { get; set; }
    }
}
