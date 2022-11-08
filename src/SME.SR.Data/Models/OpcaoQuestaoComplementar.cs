namespace SME.SR.Data
{
    public class OpcaoQuestaoComplementar
    {
        public long OpcaoRespostaId { get; set; }
        public OpcaoResposta OpcaoResposta { get; set; }
        public Questao QuestaoComplementar { get; set; }
        public long QuestaoComplementarId { get; set; }        
    }
}