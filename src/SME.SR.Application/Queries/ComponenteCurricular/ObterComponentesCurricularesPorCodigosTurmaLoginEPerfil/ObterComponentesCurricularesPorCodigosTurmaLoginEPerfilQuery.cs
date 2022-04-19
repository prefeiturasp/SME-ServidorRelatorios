using MediatR;
using SME.SR.Data;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterComponentesCurricularesPorCodigosTurmaLoginEPerfilQuery : IRequest<IEnumerable<ComponenteCurricularPorTurmaRegencia>>
    {
        public string[] CodigosTurma { get; set; }
        public IEnumerable<ComponenteCurricular> ComponentesCurriculares { get; set; }
        public IEnumerable<ComponenteCurricularGrupoMatriz> GruposMatriz { get; set; }
        public Usuario Usuario { get; set; }
        public bool ValidarAbrangenciaProfessor { get; set; }
    }
}
