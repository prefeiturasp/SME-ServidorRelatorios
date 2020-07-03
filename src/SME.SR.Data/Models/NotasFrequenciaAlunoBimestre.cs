namespace SME.SR.Data
{
    public class NotasFrequenciaAlunoBimestre
    {
        public string CodigoTurma { get; set; }

        public string CodigoAluno { get; set; }

        public string CodigoComponenteCurricular { get; set; }

        public PeriodoEscolar PeriodoEscolar { get; set; }

        public NotaConceitoBimestreComponente NotaConceito { get; set; }

        public FrequenciaAluno FrequenciaAluno { get; set; }
    }
}
