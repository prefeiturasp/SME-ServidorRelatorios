namespace SME.SR.Infra
{
    public class QuestaoEncaminhamentoNAAPADetalhadoDto
    {
        public string Questao { get; set; }
        public long QuestaoId { get; set; }
        public TipoQuestao TipoQuestao { get; set; }
        public int Ordem { get; set; }
        public string OrdemMascara { get; set; }
        public string Resposta { get; set; }
    }
}
