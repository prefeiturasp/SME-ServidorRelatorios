using MediatR;
using System.Collections.Generic;

namespace SME.SR.Application.Queries.HistoricoEscolar.ObterHistoricoEscolar
{
    public class ObterHistoricoEscolarQuery : IRequest<IEnumerable<Data.HistoricoEscolar>>
    {
        public string CodigoTurma { get; set; }
        public string CodigoAluno { get; set; }
    }
}
