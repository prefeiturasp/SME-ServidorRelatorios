namespace SME.SR.Infra
{
    public class PerguntaRelatorioMatematicaNumerosDto
    {
        public string PerguntaId { get; set; }
        public string PerguntaDescricao { get; set; }
        public string RespostaId { get; set; }
        public string RespostaDescricao { get; set; }
        public string CodigoDre { get; set; }
        public string CodigoUe { get; set; }
        public string CodigoTurma { get; set; }
        public int AnoTurma { get; set; }
        public int QtdRespostas { get; set; }
        public int OrdemReposta { get; set; }
        public int OrdemPergunta { get; set; }
    }
}
