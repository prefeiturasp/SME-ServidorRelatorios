using MediatR;
using SME.SR.Data;

namespace SME.SR.Application
{
    public class ObterTurmaPorCodigoQuery : IRequest<Turma>
    {
        public ObterTurmaPorCodigoQuery(string turmaCodigo)
        {
            TurmaCodigo = turmaCodigo;
        }

        public string TurmaCodigo { get; }
    }
}
