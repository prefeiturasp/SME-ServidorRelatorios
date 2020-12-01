using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioAlteracaoNotasDto
    {
        public RelatorioAlteracaoNotasDto()
        {
            Filtro = new FiltroAlteracaoNotasDto();
            Turmas = new List<TurmaAlteracaoNotasDto>();
        }
                
        public FiltroAlteracaoNotasDto Filtro { get; set; }
        public List<TurmaAlteracaoNotasDto> Turmas { get; set; }
    }
}
