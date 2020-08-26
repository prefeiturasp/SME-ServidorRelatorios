namespace SME.SR.Infra
{
    public class RelatorioCompensacaoAusenciaRetornoConsulta
    {
        public long DisciplinaId { get; set; }
        public int Bimestre { get; set; }
        public string AtividadeNome { get; set; }
        public long TurmaId { get; set; }
        public string TurmaCodigo { get; set; }
        public int FaltasCompensadas { get; set; }
        public long AlunoCodigo { get; set; }
        public string TurmaNome { get; set; }
    }
}
