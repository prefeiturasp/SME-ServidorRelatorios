using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class RelatorioBoletimSimplesEscolarDto
    {

        public bool EhRegencia { get; set; }
        public Modalidade ModalidadeTurma { get; set; }
        public BoletimEscolarCabecalhoDto Cabecalho { get; set; }
        public ComponenteCurricularRegenciaDto ComponenteCurricularRegencia { get; set; }
        public List<ComponenteCurricularDto> ComponentesCurriculares { get; set; }
        public string ParecerConclusivo { get; set; }
    }
}
