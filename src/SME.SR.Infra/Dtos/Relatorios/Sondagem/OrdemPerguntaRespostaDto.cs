namespace SME.SR.Infra
{
    public class OrdemPerguntaRespostaDto
    {
        public string OrdermId { get; set; }
        public string Ordem { get; set; }
        public string PerguntaId { get; set; }
        public string PerguntaDescricao { get; set; }
        public string RespostaId { get; set; }
        public string RespostaDescricao { get; set; }
        public int QtdRespostas { get; set; }
        public string CodigoTurma { get; set; }
    }
}