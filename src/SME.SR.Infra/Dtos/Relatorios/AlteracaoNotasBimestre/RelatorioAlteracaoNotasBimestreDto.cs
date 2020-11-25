using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioAlteracaoNotasBimestreDto
    {
        public RelatorioAlteracaoNotasBimestreDto()
        {
            Filtro = new FiltroAlteracaoNotasBimestreDto();
            Turmas = new List<TurmaAlteracaoNotasBimestreDto>();
        }

        public FiltroAlteracaoNotasBimestreDto Filtro { get; set; }

        public List<TurmaAlteracaoNotasBimestreDto> Turmas { get; set; }
    }
}
