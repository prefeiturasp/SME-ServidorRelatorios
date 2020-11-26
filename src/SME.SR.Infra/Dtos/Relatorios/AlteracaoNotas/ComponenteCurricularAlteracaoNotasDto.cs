using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class ComponenteCurricularAlteracaoNotasDto
    {
        public ComponenteCurricularAlteracaoNotasDto()
        {
            AlunosAlteracaoNotasBimestre = new List<AlunosAlteracaoNotasDto>();
        }
        public string Nome { get; set; }
        public IEnumerable<AlunosAlteracaoNotasDto> AlunosAlteracaoNotasBimestre { get; set; }
    }
}
