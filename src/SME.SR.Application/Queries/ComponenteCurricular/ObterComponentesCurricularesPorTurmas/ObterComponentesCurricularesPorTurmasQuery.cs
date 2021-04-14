using MediatR;
using SME.SR.Data;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public  class ObterComponentesCurricularesPorTurmasQuery : IRequest<IEnumerable<ComponenteCurricularPorTurma>>
    {
        public ObterComponentesCurricularesPorTurmasQuery() { }
        public ObterComponentesCurricularesPorTurmasQuery(string[] codigosTurma)
        {
            CodigosTurma = codigosTurma;
        }

        public string[] CodigosTurma { get; set; }
    }
}
