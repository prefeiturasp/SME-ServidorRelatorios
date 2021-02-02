using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class TurmaControleGradeDto
    {
        public TurmaControleGradeDto()
        {
            Bimestres = new List<BimestreControleGradeDto>();
        }

        public long Id { get; set; }
        public string Nome { get; set; }
        public List<BimestreControleGradeDto> Bimestres { get; set; }
    }
}
