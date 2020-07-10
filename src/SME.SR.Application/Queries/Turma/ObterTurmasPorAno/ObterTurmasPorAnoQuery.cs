using MediatR;
using SME.SR.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Application
{
    public class ObterTurmasPorAnoQuery: IRequest<IEnumerable<Turma>>
    {
        public ObterTurmasPorAnoQuery(int anoLetivo, IEnumerable<string> anosEscolares)
        {
            AnoLetivo = anoLetivo;
            AnosEscolares = anosEscolares;
        }

        public int AnoLetivo { get; set; }
        public IEnumerable<string> AnosEscolares { get; set; }
    }
}
