using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class BimestreAlteracaoNotasBimestreDto
    {
        public BimestreAlteracaoNotasBimestreDto()
        {
            ComponentesCurriculares = new List<ComponenteCurricularAlteracaoNotasBimestreDto>();
        }
        public string Descricao { get; set; }
        public List<ComponenteCurricularAlteracaoNotasBimestreDto> ComponentesCurriculares { get; set; }
    }
}
