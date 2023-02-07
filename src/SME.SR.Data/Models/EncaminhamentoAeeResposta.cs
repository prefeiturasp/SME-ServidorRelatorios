namespace SME.SR.Data
{
    public class EncaminhamentoAeeResposta
    {
        public long Id { get; set; }
        public EncaminhamentoAeeQuestao EncaminhamentoAeeQuestao { get; set; }
        public long EncaminhamentoAeeQuestaoId { get; set; }
        public OpcaoResposta Resposta { get; set; }
        public long RespostaId { get; set; }
        public string Texto { get; set; }
    }
}