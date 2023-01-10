namespace SME.SR.Data
{
    public class PlanoAeeResposta
    {
        public long Id { get; set; }
        public PlanoAeeQuestao PlanoAeeQuestao { get; set; }
        public long PlanoAeeQuestaoId { get; set; }
        public OpcaoResposta Resposta { get; set; }
        public long RespostaId { get; set; }
        public string Texto { get; set; }
    }
}