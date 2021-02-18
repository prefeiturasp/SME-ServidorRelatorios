using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class AlunoSituacaoDto
    {
        public long CodigoAluno { get; set; }
        public string NomeAluno { get; set; }
        public SituacaoMatriculaAluno CodigoSituacaoMatricula { get; set; }
        public string SituacaoMatricula { get; set; }
        public string NumeroAlunoChamada { get; set; }
        public DateTime DataSituacaoAluno { get; set; }
    }
}
