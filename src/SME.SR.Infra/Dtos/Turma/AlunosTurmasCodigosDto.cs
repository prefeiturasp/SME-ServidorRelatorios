namespace SME.SR.Infra
{
    public class AlunosTurmasCodigosDto
    {
        public long AlunoCodigo { get; set; }
        public string TurmaCodigo { get; set; }
        public string RegularCodigo { get; set; }
        public Modalidade Modalidade { get; set; }
        public string Ano { get; set; }
        public int EtapaEJA { get; set; }
        public string Ciclo { get; set; }
        public string ParecerConclusivo { get; set; }
        public TipoTurma TipoTurma { get; set; }
        public string DescricaoAno => Modalidade == Modalidade.EJA ? $"{Ciclo} - {(EtapaEJA == 1 ? "I" : "II")}" : $"{Ano}º ano";
        public int SemestreTurma { get; set; }
        public int SemestreTurmaRegular { get; set; }
    }
}