using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class ControleGradeDto
    {
        public ControleGradeDto()
        {
            Filtro = new FiltroControleGrade();
            Turmas = new List<TurmaControleGradeDto>();
        }

        public FiltroControleGrade Filtro { get; set; }
       
        public List<TurmaControleGradeDto> Turmas { get; set; }
    }
}
