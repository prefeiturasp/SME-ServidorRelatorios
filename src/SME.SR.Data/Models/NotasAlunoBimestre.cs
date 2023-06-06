using SME.SR.Infra;

namespace SME.SR.Data
{
    public class NotasAlunoBimestre
    {
        public long ConselhoClasseAlunoId { get; set; }
        public long IdTurma { get; set; }
        public string CodigoTurma { get; set; }
        public TipoTurma TipoTurma { get; set; }

        public long TurmaComplementarId { get; set; }
        public string CodigoAluno { get; set; }

        public string CodigoComponenteCurricular { get; set; }

        public bool Aprovado { get; set; }

        public PeriodoEscolar PeriodoEscolar { get; set; }

        public NotaConceitoBimestreComponente NotaConceito { get; set; }
    }
}
