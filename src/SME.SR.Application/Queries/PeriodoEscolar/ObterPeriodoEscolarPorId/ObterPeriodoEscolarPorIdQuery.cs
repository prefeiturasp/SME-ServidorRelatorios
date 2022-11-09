using MediatR;
using SME.SR.Data;

namespace SME.SR.Application
{
    public class ObterPeriodoEscolarPorIdQuery : IRequest<PeriodoEscolar>
    {
        public ObterPeriodoEscolarPorIdQuery(long idPeriodoEscolar)
        {
            IdPeriodoEscolar = idPeriodoEscolar;
        }

        public long IdPeriodoEscolar { get; }
    }
}