using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.Dtos.Relatorios.BoletimEscolar
{
    public class BoletimEscolarDto
    {
        public List<BoletimEscolarAlunoDto> Alunos { get; set; }

        public BoletimEscolarDto()
        {
            Alunos = new List<BoletimEscolarAlunoDto>();
        }
    }
}
