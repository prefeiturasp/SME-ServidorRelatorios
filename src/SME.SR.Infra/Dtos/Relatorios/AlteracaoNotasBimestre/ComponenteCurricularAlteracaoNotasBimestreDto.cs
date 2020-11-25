using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class ComponenteCurricularAlteracaoNotasBimestreDto
    {
        public ComponenteCurricularAlteracaoNotasBimestreDto()
        {
            AlunosAlteracaoNotasBimestre = new List<AlunosAlteracaoNotasBimestreDto>();
        }
        public string Nome { get; set; }
        public IEnumerable<AlunosAlteracaoNotasBimestreDto> AlunosAlteracaoNotasBimestre { get; set; }
    }
}
