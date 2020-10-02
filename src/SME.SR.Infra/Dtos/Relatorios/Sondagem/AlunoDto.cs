using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class AlunoDto
    {
        public long Codigo { get; set; }
        public string Nome { get; set; }
        public SituacaoMatriculaAluno SituacaoMatricula { get; set; }
    }
}
