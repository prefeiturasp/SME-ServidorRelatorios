namespace SME.SR.Workers.SGP.Models
{
    public class FechamentoTurma
    {
        public string TurmaId { get; set; }
        public Turma Turma { get; set; }
        public long? PeriodoEscolarId { get; set; }
        public PeriodoEscolar PeriodoEscolar { get; set; }
    }
}
