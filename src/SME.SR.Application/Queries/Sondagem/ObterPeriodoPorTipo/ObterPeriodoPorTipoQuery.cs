using MediatR;
using SME.SR.Data;
using SME.SR.Infra;

namespace SME.SR.Application
{
    public class ObterPeriodoPorTipoQuery : IRequest<PeriodoSondagem>
    {
        public ObterPeriodoPorTipoQuery(int periodo, TipoPeriodoSondagem tipoPeriodo)
        {
            Periodo = periodo;
            TipoPeriodo = tipoPeriodo;
        }

        public int Periodo { get; set; }

        public TipoPeriodoSondagem TipoPeriodo { get; set; }
    }
}
