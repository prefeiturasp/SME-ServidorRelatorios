using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.Dtos.Relatorios.BoletimEscolar
{
    public class GrupoMatrizComponenteCurricularDto
    {
        public int Id{ get; set; }

        public string Nome { get; set; }

        public string Descricao { get; set; }

        public List<ComponenteCurricularDto> ComponentesCurriculares { get; set; }
    }
}
