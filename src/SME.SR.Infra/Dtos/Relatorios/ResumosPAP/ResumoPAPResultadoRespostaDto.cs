namespace SME.SR.Infra
{
    public class ResumoPAPResultadoRespostaDto
    {
        public ResumoPAPResultadoRespostaDto()
        {
            Porcentagem = 0;
            Quantidade = 0;
            TotalPorcentagem = 0;
            TotalQuantidade = 0;
            Ordem = 0;
        }
        public double Porcentagem { get; set; }
        public int Quantidade { get; set; }
        public string RespostaDescricao { get; set; }
        public string RespostaNome { get; set; }
        public int TotalPorcentagem { get; set; }
        public int TotalQuantidade { get; set; }
        public int Ordem { get; set; }
        public long RespostaId { get; set; }
    }
}
