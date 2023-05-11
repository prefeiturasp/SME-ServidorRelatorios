namespace SME.SR.Infra.Dtos.FrequenciaMensal
{
    public class ControleFrequenciaPorAulaDto
    {
        public string DiaSemanaSigla { get; set; }
        public int DiaSemanaNumero { get; set; }
        public int TotalAulas { get; set; }
        public int TotalPresenca { get; set; }
        public int TotalCompesacao { get; set; }
    }
}