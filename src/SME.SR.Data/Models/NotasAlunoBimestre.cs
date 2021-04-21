namespace SME.SR.Data
{
    public class NotasAlunoBimestre
    {
        public string CodigoTurma { get; set; }

        public string CodigoAluno { get; set; }

        public string CodigoComponenteCurricular { get; set; }

        public bool Aprovado { get; set; }

        public PeriodoEscolar PeriodoEscolar { get; set; }

        public NotaConceitoBimestreComponente NotaConceito { get; set; }
    }
}
