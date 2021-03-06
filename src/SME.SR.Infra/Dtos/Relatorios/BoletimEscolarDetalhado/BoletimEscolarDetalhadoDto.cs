﻿using System.Collections.Generic;
using System.Linq;

namespace SME.SR.Infra
{
    public class BoletimEscolarDetalhadoDto
    {
        public List<BoletimEscolarDetalhadoAlunoDto> Boletins { get; set; }

        public BoletimEscolarDetalhadoDto()
        {
            Boletins = new List<BoletimEscolarDetalhadoAlunoDto>();
        }

        public BoletimEscolarDetalhadoDto(List<BoletimEscolarDetalhadoAlunoDto> boletins)
        {
            if (boletins != null && boletins.Any())
                this.Boletins = boletins;
        }
    }
}
