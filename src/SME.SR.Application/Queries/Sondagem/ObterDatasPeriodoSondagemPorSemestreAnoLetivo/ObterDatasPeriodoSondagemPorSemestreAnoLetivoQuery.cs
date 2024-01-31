using MediatR;
using SME.SR.Infra;
using System;

namespace SME.SR.Application
{
    public class ObterDatasPeriodoSondagemPorSemestreAnoLetivoQuery : IRequest<PeriodoCompletoSondagemDto>
    {
        public ObterDatasPeriodoSondagemPorSemestreAnoLetivoQuery(int semestre, int anoLetivo)
        {
            Semestre = semestre;
            AnoLetivo = anoLetivo;
        }

        public int Semestre { get; set; }
        public int AnoLetivo { get; set; }
    }
}
