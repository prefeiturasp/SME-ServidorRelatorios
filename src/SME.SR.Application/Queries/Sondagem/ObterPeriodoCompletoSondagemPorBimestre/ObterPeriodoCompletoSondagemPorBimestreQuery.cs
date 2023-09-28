using MediatR;
using SME.SR.Infra;
using SME.SR.Infra.Dtos.Sondagem;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Application
{
    public class ObterPeriodoCompletoSondagemPorBimestreQuery : IRequest<PeriodoCompletoSondagemDto>
    {
        public ObterPeriodoCompletoSondagemPorBimestreQuery(int bimestre, int anoLetivo)
        {
            Bimestre = bimestre;
            AnoLetivo = anoLetivo;
        }

        public int Bimestre { get; set; }
        public int AnoLetivo { get; set; }
    }
}
