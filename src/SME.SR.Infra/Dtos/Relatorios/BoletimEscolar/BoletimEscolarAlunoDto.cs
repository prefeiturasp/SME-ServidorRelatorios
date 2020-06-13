using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.Dtos.Relatorios.BoletimEscolar
{
    public class BoletimEscolarAlunoDto
    {
        public string DescricaoGrupos { get; set; }

        public BoletimEscolarCabecalhoDto Cabecalho { get; set; }

        public List<ComponenteCurricularDto> ComponentesCurriculares { get; set; }

        public BoletimEscolarAlunoDto()
        {
            Cabecalho = new BoletimEscolarCabecalhoDto();
            ComponentesCurriculares = new List<ComponenteCurricularDto>();
        }
    }
}
