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
        public ObterDadosPedagogicosComponenteCurricularesQuery(long[] componentesCurriculares, int anoLetivo, List<string> codigoTurmas)
        {
            ComponentesCurriculares = componentesCurriculares;
            Turmas = codigoTurmas;
            AnoLetivo = anoLetivo;
        }

        public long[] ComponentesCurriculares { get; set; }
        public int AnoLetivo { get; set; }
        public List<string> Turmas { get; set; }
    }
}
