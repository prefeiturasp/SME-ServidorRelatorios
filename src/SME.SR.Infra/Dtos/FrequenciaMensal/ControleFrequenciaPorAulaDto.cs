namespace SME.SR.Infra.Dtos.FrequenciaMensal
{
    public class ControleFrequenciaPorAulaDto
    {
        public string DiaSemanaSigla { get; set; }
        public int DiaSemanaNumero { get; set; }
        public int Valor { get; set; } = 0;
    }
}