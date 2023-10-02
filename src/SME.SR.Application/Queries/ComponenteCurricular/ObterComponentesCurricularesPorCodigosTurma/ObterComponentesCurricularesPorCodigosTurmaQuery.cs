using MediatR;
using SME.SR.Data;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterComponentesCurricularesPorCodigosTurmaQuery : IRequest<IEnumerable<ComponenteCurricularPorTurmaRegencia>>
    {
        public string[] CodigosTurma { get; set; }
        public IEnumerable<ComponenteCurricular> ComponentesCurriculares { get; set; }
        public IEnumerable<ComponenteCurricularGrupoMatriz> GruposMatriz { get; set; }
        public bool EhEJA { get; set; }


        public ObterComponentesCurricularesPorCodigosTurmaQuery(string[] codigosTurma, IEnumerable<ComponenteCurricular> componentesCurriculares, IEnumerable<ComponenteCurricularGrupoMatriz> gruposMatriz, bool ehEJA = false)
        {
            CodigosTurma = codigosTurma;
            ComponentesCurriculares = componentesCurriculares;
            GruposMatriz = gruposMatriz;
            EhEJA = ehEJA;
        }

        public ObterComponentesCurricularesPorCodigosTurmaQuery()
        {
            
        }
    }
}
