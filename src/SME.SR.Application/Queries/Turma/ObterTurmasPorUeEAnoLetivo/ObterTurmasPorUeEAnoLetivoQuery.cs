using MediatR;
using SME.SR.Data;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterTurmasPorUeEAnoLetivoQuery : IRequest<IEnumerable<Turma>>
    {
        public ObterTurmasPorUeEAnoLetivoQuery(string codigoUE, long anoLetivo)
        {
            CodigoUE = codigoUE;
            AnoLetivo = anoLetivo;
        }

        public string CodigoUE { get; set; }
        public long AnoLetivo { get; set; }
    }
}
