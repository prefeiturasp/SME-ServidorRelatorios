using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class TurmaAlteracaoNotasDto
    {
        public TurmaAlteracaoNotasDto()
        {
            Bimestres = new List<BimestreAlteracaoNotasDto>();
        }

        public long Id { get; set; }
        public string Nome { get; set; }
        public List<BimestreAlteracaoNotasDto> Bimestres { get; set; }
    }
}
