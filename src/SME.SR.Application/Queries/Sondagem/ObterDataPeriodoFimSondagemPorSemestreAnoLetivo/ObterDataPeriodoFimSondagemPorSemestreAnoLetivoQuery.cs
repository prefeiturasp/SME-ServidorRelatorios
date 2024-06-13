using MediatR;
using SME.SR.Infra;
using System;

namespace SME.SR.Application
{
    public class ObterDataPeriodoFimSondagemPorSemestreAnoLetivoQuery : IRequest<PeriodoCompletoSondagemDto>
    {
        public ObterDataPeriodoFimSondagemPorSemestreAnoLetivoQuery(int semestre, int anoLetivo)
        {
            Semestre = semestre;
            AnoLetivo = anoLetivo;
        }

        public int Semestre { get; set; }
        public int AnoLetivo { get; set; }
    }
}
