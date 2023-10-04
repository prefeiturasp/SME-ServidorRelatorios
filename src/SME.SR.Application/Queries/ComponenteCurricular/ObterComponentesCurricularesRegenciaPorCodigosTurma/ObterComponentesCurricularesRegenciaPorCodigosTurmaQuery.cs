using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System.Collections.Generic;
using System.Linq;

namespace SME.SR.Application
{
    public class ObterComponentesCurricularesRegenciaPorCodigosTurmaQuery : IRequest<IEnumerable<IGrouping<string, ComponenteCurricularPorTurmaRegencia>>>
    {
        public string[] CodigosTurma { get; set; }
        public long[] CdComponentesCurriculares { get; set; }
        public string CodigoUe { get; set; }
        public Modalidade Modalidade { get; set; }
        public IEnumerable<ComponenteCurricular> ComponentesCurriculares { get; set; }
        public IEnumerable<ComponenteCurricularGrupoMatriz> GruposMatriz { get; set; }
        public Usuario Usuario { get; set; }
    }
}
