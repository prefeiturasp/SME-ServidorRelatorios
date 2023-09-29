using MediatR;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Application
{
    public class ObterPeriodoCompletoSondagemPorSemestreQuery : IRequest<PeriodoCompletoSondagemDto>
    {
        public int Semestre { get; set; }
        public string AnoLetivo { get; set; }

        public ObterPeriodoCompletoSondagemPorSemestreQuery(int semestre, string anoLetivo)
        {
            Semestre = semestre;
            AnoLetivo = anoLetivo;
        }
    }
}
