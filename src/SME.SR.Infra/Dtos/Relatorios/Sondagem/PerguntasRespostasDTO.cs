namespace SME.SR.Infra.Dtos.Relatorios.Sondagem
{
    public class PerguntasRespostasDTO
    {
        public string PerguntaId { get; set; }
        public string PerguntaDescricao { get; set; }
        public string RespostaId { get; set; }
        public string RespostaDescricao { get; set; }
        public int QtdRespostas { get; set; }
    }
}
