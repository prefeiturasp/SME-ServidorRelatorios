using MediatR;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Application
{
    public class ObterDadosPedagogicosComponenteCurricularesQuery : IRequest<IEnumerable<ComponenteCurricularPedagogicoDto>>
    {
        public ObterDadosPedagogicosComponenteCurricularesQuery() { }
        public ObterDadosPedagogicosComponenteCurricularesQuery(long[] componentesCurriculares, int anoLetivo, long[] codigoTurmas)
        {
            ComponentesCurriculares = componentesCurriculares;
            TurmasId = codigoTurmas;
            AnoLetivo = anoLetivo;
        }

        public long[] ComponentesCurriculares { get; set; }
        public int AnoLetivo { get; set; }
        public long[] TurmasId { get; set; }
    }
}
