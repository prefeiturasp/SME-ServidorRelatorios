using MediatR;
using SME.SR.Data;

namespace SME.SR.Application
{
    public class ObterFechamentoTurmaPorIdQuery : IRequest<FechamentoTurma>
    {
        public long FechamentoTurmaId { get; set; }
    }
}
