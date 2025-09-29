using System;
using System.Linq;

namespace SME.SR.Infra
{
    public class DadosMatriculaAlunoDto
    {
        public string ObterNomeFinal()
        {
            if (string.IsNullOrEmpty(NomeSocialAluno))
                return NomeAluno;
            else return NomeSocialAluno;
        }
        public long CodigoAluno { get; set; }
        public string CodigoTurma { get; set; }
        public int CodigoSituacaoMatricula { get; set; }
        public string NomeAluno { get; set; }
        public string NomeSocialAluno { get; set; }
        public DateTime DataSituacao { get; set; }
        public DateTime DataMatricula { get; set; }
        public int AnoLetivo { get; set; }
        public string NumeroAlunoChamada { get; set; }
        public string SituacaoMatricula { get; set; }
        public bool Ativo { get { return SituacoesAtivo.Contains(CodigoSituacaoMatricula); } }
        public bool Inativo { get { return !Ativo; } }
        private int[] SituacoesAtivo => new[] { (int)SituacaoMatriculaAluno.Ativo,
                                                (int)SituacaoMatriculaAluno.Rematriculado,
                                                (int)SituacaoMatriculaAluno.PendenteRematricula,
                                                (int)SituacaoMatriculaAluno.SemContinuidade,
                                                (int)SituacaoMatriculaAluno.Concluido };
    }
}