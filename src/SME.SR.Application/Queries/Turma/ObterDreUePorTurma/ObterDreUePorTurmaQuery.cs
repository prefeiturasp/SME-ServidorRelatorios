using MediatR;
using SME.SR.Data;

namespace SME.SR.Application
{
    public class ObterDreUePorTurmaQuery : IRequest<DreUe>
    {
        public string CodigoTurma { get; set; }
    }
}
