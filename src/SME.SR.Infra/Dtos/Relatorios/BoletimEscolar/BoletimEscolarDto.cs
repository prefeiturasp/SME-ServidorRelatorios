using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.Dtos.Relatorios.BoletimEscolar
{
    public class BoletimEscolarDto
    {
        public List<BoletimEscolarAlunoDto> Boletins { get; set; }

        public BoletimEscolarDto()
        {
            Boletins = new List<BoletimEscolarAlunoDto>();
        }
    }
}
