namespace SME.SR.Infra
{
    public class RelatorioSondagemComponentesPorTurmaRetornoQueryDto
    {
        public RelatorioSondagemComponentesPorTurmaRetornoQueryDto(int dreId, int turmaId, int ueId, int ano)
        {
            this.DreId = dreId;
            this.TurmaId = turmaId;
            this.UeId = ueId;
            this.Ano = ano;
        }
        public int DreId { get; set; }
        public int TurmaId { get; set; }
        public int UeId { get; set; }
        public int Ano { get; set; }
    }
}
