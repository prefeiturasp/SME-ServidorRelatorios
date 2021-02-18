using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class BimestreAlteracaoNotasDto
    {
        public BimestreAlteracaoNotasDto()
        {
            ComponentesCurriculares = new List<ComponenteCurricularAlteracaoNotasDto>();
        }
        public string Descricao { get; set; }
        public List<ComponenteCurricularAlteracaoNotasDto> ComponentesCurriculares { get; set; }
    }
}
