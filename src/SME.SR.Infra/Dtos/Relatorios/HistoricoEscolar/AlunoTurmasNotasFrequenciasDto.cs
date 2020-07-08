using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class AlunoTurmasNotasFrequenciasDto
    {
        public InformacoesAlunoDto Aluno { get; set; }
        public long[] Turmas { get; set; }
    }
}
