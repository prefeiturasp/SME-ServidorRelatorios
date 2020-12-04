using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class AtribuicaoCjPorTurmaDto
    {
        public string NomeTurma { get; set; }
        public List<AtribuicaoCjProfessorDto> AtribuicoesCjProfessor { get; set; }
    }
}
