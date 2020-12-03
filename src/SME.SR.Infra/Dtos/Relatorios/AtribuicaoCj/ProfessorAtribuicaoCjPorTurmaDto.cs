using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class ProfessorAtribuicaoCjPorTurmaDto
    {
        public string NomeProfessor { get; set; }
        public List<AtribuicaoCjTurmaDto> AtribuiicoesCjTurma { get; set; }
    }
}
