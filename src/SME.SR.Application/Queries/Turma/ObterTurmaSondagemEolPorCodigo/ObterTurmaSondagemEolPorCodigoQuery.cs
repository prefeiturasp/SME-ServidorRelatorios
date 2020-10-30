using MediatR;
using SME.SR.Data;

namespace SME.SR.Application
{
    public class ObterTurmaSondagemEolPorCodigoQuery : IRequest<Turma>
    {
        public ObterTurmaSondagemEolPorCodigoQuery(long turmaCodigo)
        {
            TurmaCodigo = turmaCodigo;
        }

        public long TurmaCodigo { get; set; }
    }
}
