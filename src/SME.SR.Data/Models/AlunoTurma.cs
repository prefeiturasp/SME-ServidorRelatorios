using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
using System;

namespace SME.SR.Data
{
    public class AlunoTurma
    {
        public string NumeroChamada { get; set; }
        public string Nome { get; set; }
        public string TurmaCodigo { get; set; }
        public int CodigoAluno { get; set; }
        public string NomeFinal { get; set; }
        public DateTime DataSituacao { get; set; }
        public SituacaoMatriculaAluno SituacaoMatricula { get; set; }
        public bool Ativo => SituacaoMatricula.EhUmDosValores(SituacaoMatriculaAluno.Ativo,
                                                              SituacaoMatriculaAluno.Concluido,
                                                              SituacaoMatriculaAluno.PendenteRematricula,
                                                              SituacaoMatriculaAluno.Rematriculado,
                                                              SituacaoMatriculaAluno.SemContinuidade);
    }
}
