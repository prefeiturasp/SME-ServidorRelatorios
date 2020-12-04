using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class AtribuicaoCjPorProfessorDto
    {
        public string NomeProfessor { get; set; }
        public List<AtribuicaoCjTurmaDto> AtribuiicoesCjTurma { get; set; }
    }
}
