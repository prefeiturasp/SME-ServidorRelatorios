namespace SME.SR.Infra
{
    public class FrequenciaMensalDto
    {
        public string CodigoDre { get; set; }
        public string NomeDre { get; set; }
        public string CodigoUe { get; set; }
        public string NomeUe { get; set; }
        public string NomeTurma { get; set; }
        public string CodigoTurma { get; set; }
        public string NomeMes { get; set; }
        public int ValorMes { get; set; }
        public string CodigoAluno { get; set; }
        public string NumeroAluno { get; set; }
        public string NomeAluno { get; set; }
        public decimal? ProcentagemFrequencia { get; set; }
    }
}
