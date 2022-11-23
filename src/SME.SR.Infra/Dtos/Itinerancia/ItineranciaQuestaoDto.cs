namespace SME.SR.Infra
{
    public class ItineranciaQuestaoDto
    {
        public long Id { get; set; }
        public long ItineranciaId { get; set; }
        public long? ItineranciaAlunoId { get; set; }
        public int Ordem { get; set; }
        public string Nome { get; set; }
        public string Resposta { get; set; }
        public TipoQuestao TipoQuestao { get; set; }
    }
}
