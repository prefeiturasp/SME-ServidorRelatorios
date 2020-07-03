using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterCabecalhoHistoricoEscolarQuery : IRequest<CabecalhoDto>
    {
        public string codigoDre { get; set; }
    }
}
