using MediatR;
using SME.SR.Workers.SGP.Models;

namespace SME.SR.Workers.SGP.Queries
{
    public class ObterDreUePorTurmaQuery : IRequest<DreUe>
    {
        public string CodigoTurma { get; set; }
    }
}
