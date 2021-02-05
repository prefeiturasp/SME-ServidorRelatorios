using MediatR;
using System;

namespace SME.SR.Application.Queries
{
    public class ObterDataPeriodoFimSondagemPorBimestreAnoLetivoQuery : IRequest<DateTime>
    {
        public ObterDataPeriodoFimSondagemPorBimestreAnoLetivoQuery(int bimestre, int anoLetivo)
        {
            Bimestre = bimestre;
            AnoLetivo = anoLetivo;
        }

        public int Bimestre { get; set; }
        public int AnoLetivo { get; set; }
    }
}
