using System;

namespace SME.SR.Infra
{
    public class AlunosNaTurmaDto
    {
        public int CodigoAluno { get; set; }
        public string NomeAluno { get; set; }
        public DateTime DataNascimento { get; set; }
        public string NomeSocialAluno { get; set; }
        public int CodigoSituacaoMatricula { get; set; }
        public string SituacaoMatricula { get; set; }
        public DateTime DataSituacao { get; set; }
        public string NumeroAlunoChamada { get; set; }
    }
}