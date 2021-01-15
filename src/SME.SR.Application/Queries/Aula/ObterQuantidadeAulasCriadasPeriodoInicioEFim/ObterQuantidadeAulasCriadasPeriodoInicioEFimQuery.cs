using MediatR;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Application
{
    public class ObterQuantidadeAulasCriadasPeriodoInicioEFimQuery : IRequest<int>
    {
        public ObterQuantidadeAulasCriadasPeriodoInicioEFimQuery(long turmaId, long componenteCurricularId, DateTime dataInicio, DateTime dataFim)
        {
            TurmaId = turmaId;
            ComponenteCurricularId = componenteCurricularId;
            DataInicio = dataInicio;
            DataFim = dataFim;
        }

        public long TurmaId { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public long ComponenteCurricularId { get; set; }
    }
}
