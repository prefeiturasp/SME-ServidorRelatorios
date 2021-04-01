using MediatR;
using SME.SR.Data;

namespace SME.SR.Application.Queries.BoletimEscolar
{
    public class ObterFehamentoPorCodigoTurmaQuery : IRequest<FechamentoTurma>
    {
        public string CodigoTurma { get; set; }
    }
}
