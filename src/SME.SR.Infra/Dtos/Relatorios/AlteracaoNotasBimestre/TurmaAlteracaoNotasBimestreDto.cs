using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class TurmaAlteracaoNotasBimestreDto
    {
        public TurmaAlteracaoNotasBimestreDto()
        {
            Bimestres = new List<BimestreControleGradeDto>();
        }

        public long Id { get; set; }
        public string Nome { get; set; }
        public List<BimestreControleGradeDto> Bimestres { get; set; }
    }
}
