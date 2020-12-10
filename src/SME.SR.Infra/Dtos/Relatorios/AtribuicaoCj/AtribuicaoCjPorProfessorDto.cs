using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class AtribuicaoCjPorProfessorDto
    {
        public AtribuicaoCjPorProfessorDto()
        {
            AtribuiicoesCjTurma = new List<AtribuicaoCjTurmaDto>();
        }

        public string NomeProfessor { get; set; }
        public List<AtribuicaoCjTurmaDto> AtribuiicoesCjTurma { get; set; }
    }
}
