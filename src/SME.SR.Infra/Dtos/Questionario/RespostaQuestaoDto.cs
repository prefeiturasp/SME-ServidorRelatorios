namespace SME.SR.Infra
{
    public class RespostaQuestaoDto
    {
        public long Id { get; set; }
        public long SecaoId { get; set; }
        public long? OpcaoRespostaId { get; set; }
        public long QuestaoId { get; set; }
        public string Texto { get; set; }
    }
}