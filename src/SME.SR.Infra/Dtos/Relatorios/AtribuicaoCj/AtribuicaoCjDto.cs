using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class AtribuicaoCjDto
    {
        public List<TurmaAtribuicaoCjPorProfessorDto> TurmasAtribuicoesCjPorProfessor { get; set; }
        public List<ProfessorAtribuicaoCjPorTurmaDto> ProfessoresAtribuicoesCjPorTurma { get; set; }
    }
}
