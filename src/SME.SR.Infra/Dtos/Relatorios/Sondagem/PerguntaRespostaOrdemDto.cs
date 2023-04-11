namespace SME.SR.Infra
{
    public class PerguntaRespostaOrdemDto
    {
        public string CodigoTurma { get; set; }
        public string AnoTurma { get; set; }
        public string CodigoDre { get; set; }
        public string CodigoUe { get; set; }
        public int OrdemPergunta { get; set; }       
        public string PerguntaId { get; set; }
        public string PerguntaDescricao { get; set; }
        public string SubPerguntaId { get; set; }
        public string SubPerguntaDescricao { get; set; }
        public int OrdemResposta { get; set; }
        public string RespostaId { get; set; }
        public string RespostaDescricao { get; set; }
        public int QtdRespostas { get; set; }
    }
}