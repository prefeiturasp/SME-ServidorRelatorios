using MediatR;
using SME.SR.Data;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterComponentesCurricularesPorTurmaQuery : IRequest<IEnumerable<ComponenteCurricularPorTurma>>
    {
        public string CodigoTurma { get; set; }
    }
}
