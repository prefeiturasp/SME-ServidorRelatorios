namespace SME.SR.Data
{
    public class FechamentoTurma
    {
        public long Id { get; set; }
        public string TurmaId { get; set; }
        public Turma Turma { get; set; }
        public long? PeriodoEscolarId { get; set; }
        public PeriodoEscolar PeriodoEscolar { get; set; }
    }
}
