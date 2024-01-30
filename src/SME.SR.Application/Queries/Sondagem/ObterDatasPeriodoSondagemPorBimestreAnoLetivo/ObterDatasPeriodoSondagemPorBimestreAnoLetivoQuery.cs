using MediatR;
using SME.SR.Infra;
using System;

namespace SME.SR.Application.Queries
{
    public class ObterDatasPeriodoSondagemPorBimestreAnoLetivoQuery : IRequest<PeriodoCompletoSondagemDto>
    {
        public ObterDatasPeriodoSondagemPorBimestreAnoLetivoQuery(int bimestre, int anoLetivo)
        {
            Bimestre = bimestre;
            AnoLetivo = anoLetivo;
        }

        public int Bimestre { get; set; }
        public int AnoLetivo { get; set; }
    }
}
