using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class ComponenteCurricularControleGradeSinteticoDto
    {
        public long Id { get; set; }
        public string Nome { get; set; }
        public long Previstas { get; set; }
        public long CriadasProfessorTitular { get; set; }
        public long CriadasProfessorSubstituto { get; set; }
        public long DadasProfessorTitular { get; set; }
        public long DadasProfessorSubstituto { get; set; }
        public long Repostas { get; set; }
        public string Divergencias { get; set; }
    }
}
