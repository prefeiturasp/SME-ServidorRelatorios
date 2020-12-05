using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class AtribuicaoCjPorTurmaDto
    {
        public AtribuicaoCjPorTurmaDto()
        {
            AtribuicoesCjProfessor = new List<AtribuicaoCjProfessorDto>();
        }

        public string NomeTurma { get; set; }
        public List<AtribuicaoCjProfessorDto> AtribuicoesCjProfessor { get; set; }
    }
}
