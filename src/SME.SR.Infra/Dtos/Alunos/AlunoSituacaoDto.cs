using System;

namespace SME.SR.Infra
{
    public class AlunoSituacaoDto
    {
        public long CodigoAluno { get; set; }
        public long CodigoTurma { get; set; }
        public string NomeAluno { get; set; }
        public SituacaoMatriculaAluno CodigoSituacaoMatricula { get; set; }
        public string SituacaoMatricula { get; set; }
        public string NumeroAlunoChamada { get; set; }
        public DateTime DataSituacaoAluno { get; set; }
        public DateTime DataMatricula { get; set; }
    }
}
