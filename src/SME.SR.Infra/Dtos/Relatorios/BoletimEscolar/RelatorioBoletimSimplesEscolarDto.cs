using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class RelatorioBoletimSimplesEscolarDto
    {
        public BoletimEscolarCabecalhoDto Cabecalho { get; set; }

        public string ParecerConclusivo { get; set; }

        public ComponenteCurricularRegenciaDto ComponenteCurricularRegencia { get; set; }

        public List<ComponenteCurricularDto> ComponentesCurriculares { get; set; }

        public RelatorioBoletimSimplesEscolarDto()
        {
            Cabecalho = new BoletimEscolarCabecalhoDto();
        }
    }
}
