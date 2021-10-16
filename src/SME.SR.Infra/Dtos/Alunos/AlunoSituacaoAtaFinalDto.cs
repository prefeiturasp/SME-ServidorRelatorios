using System;
using System.Linq;

namespace SME.SR.Infra
{
    public class AlunoSituacaoAtaFinalDto : AlunoSituacaoDto
    {
        public AlunoSituacaoAtaFinalDto(AlunoSituacaoDto aluno)
        {

            this.CodigoAluno = aluno.CodigoAluno;
            this.NomeAluno = aluno.NomeAluno;
            this.CodigoSituacaoMatricula = aluno.CodigoSituacaoMatricula;
            this.SituacaoMatricula = aluno.SituacaoMatricula;
            this.NumeroAlunoChamada = aluno.NumeroAlunoChamada;
            this.DataSituacaoAluno = aluno.DataSituacaoAluno;
            this.DataMatricula = aluno.DataMatricula;
        }

        public bool Ativo
            => new[] { SituacaoMatriculaAluno.Ativo,
                       SituacaoMatriculaAluno.PendenteRematricula,
                       SituacaoMatriculaAluno.Rematriculado,
                       SituacaoMatriculaAluno.SemContinuidade,
                       SituacaoMatriculaAluno.Concluido }.Contains(this.CodigoSituacaoMatricula);

        public bool Inativo
            => !Ativo;
    }
}
