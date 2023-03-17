namespace SME.SR.Data
{
    public class EncaminhamentoAeeQuestao
    {
        public long Id { get; set; }
        public long EncaminhamentoAeeId { get; set; }
        public Questao Questao { get; set; }
        public long QuestaoId { get; set; }
    }
}