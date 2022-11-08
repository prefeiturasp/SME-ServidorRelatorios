namespace SME.SR.Data
{
    public class PlanoAeeQuestao
    {
        public long Id { get; set; }
        public PlanoAeeVersao PlanoAeeVersao { get; set; }
        public long PlanoAeeVersaoId { get; set; }
        public Questao Questao { get; set; }
        public long QuestaoId { get; set; }
    }
}