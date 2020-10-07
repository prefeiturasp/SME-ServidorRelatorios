using MediatR;
using SME.SR.Data;

namespace SME.SR.Application
{
    public class ObterDreUePorTurmaQuery : IRequest<DreUe>
    {
        public ObterDreUePorTurmaQuery() { }
        public ObterDreUePorTurmaQuery(string codigoTurma)
        {
            CodigoTurma = codigoTurma;
        }

        public string CodigoTurma { get; set; }
    }
}
