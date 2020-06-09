using MediatR;
using SME.SR.Workers.SGP.Models;
using System.Collections.Generic;

namespace SME.SR.Workers.SGP.Queries
{
    public class ObterComponentesCurricularesPorTurmaQuery : IRequest<IEnumerable<ComponenteCurricularPorTurma>>
    {
        public string CodigoTurma { get; set; }
    }
}
