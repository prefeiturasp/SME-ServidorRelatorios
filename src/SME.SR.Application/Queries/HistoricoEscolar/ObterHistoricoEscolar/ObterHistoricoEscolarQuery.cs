using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterHistoricoEscolarQuery : IRequest<IEnumerable<HistoricoEscolarFundamentalDto>>
    {
        public string CodigoTurma { get; set; }
        public string CodigoAluno { get; set; }
    }
}
