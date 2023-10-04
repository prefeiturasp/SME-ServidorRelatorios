using MediatR;
using SME.SR.Data;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterComponentesCurricularesPorCodigosTurmaQuery : IRequest<IEnumerable<ComponenteCurricular>>
    {
        public string[] CodigosTurma { get; set; }
        public IEnumerable<ComponenteCurricular> ComponentesCurriculares { get; set; }
        public bool EhEJA { get; set; }


        public ObterComponentesCurricularesPorCodigosTurmaQuery(string[] codigosTurma, IEnumerable<ComponenteCurricular> componentesCurriculares, bool ehEJA = false)
        {
            CodigosTurma = codigosTurma;
            ComponentesCurriculares = componentesCurriculares;
            EhEJA = ehEJA;
        }

        public ObterComponentesCurricularesPorCodigosTurmaQuery()
        {
            
        }
    }
}
