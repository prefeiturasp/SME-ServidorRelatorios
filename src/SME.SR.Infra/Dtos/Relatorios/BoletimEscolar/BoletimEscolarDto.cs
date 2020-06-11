using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.Dtos.Relatorios.BoletimEscolar
{
    public class BoletimEscolarDto
    {
        public IEnumerable<BoletimEscolarAlunoDto> Alunos { get; set; }
    }
}
