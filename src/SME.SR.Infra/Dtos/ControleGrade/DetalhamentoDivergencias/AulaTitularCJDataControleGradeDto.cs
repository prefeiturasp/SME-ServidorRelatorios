using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class AulaTitularCJDataControleGradeDto
    {
        public AulaTitularCJDataControleGradeDto()
        {
            Divergencias = new List<AulaTitularCJControleGradeDto>();
        }
        public string Data { get; set; }
        public IEnumerable<AulaTitularCJControleGradeDto> Divergencias { get; set; }
    }
}
