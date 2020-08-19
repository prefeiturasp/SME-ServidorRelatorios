using MediatR;
using SME.SR.Data;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterComponentesCurricularesPorCodigoETurmaQuery : IRequest<IEnumerable<ComponenteCurricularPorTurma>>
    {
        public ObterComponentesCurricularesPorCodigoETurmaQuery(string turmaCodigo, long[] componentesCodigo)
        {
            TurmaCodigo = turmaCodigo;
            ComponentesCodigo = componentesCodigo;
        }

        public string TurmaCodigo { get; set; }
        public long[] ComponentesCodigo { get; set; }
    }
}
