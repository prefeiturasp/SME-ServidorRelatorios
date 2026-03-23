namespace SME.SR.Infra.Dtos.Codaf
{
    public class AlunoRelatorioCodafDto
    {
        public int NumeroSequencial { get; set; }
        public string NomeAluno { get; set; }
        public string DocumentoAluno { get; set; }
        public int PercentualFrequencia { get; set; }
        public bool AtividadeObrigatoria { get; set; }
        public string ConceitoFinal { get; set; }
        public long CodigoCertificado { get; set; }
    }
}
