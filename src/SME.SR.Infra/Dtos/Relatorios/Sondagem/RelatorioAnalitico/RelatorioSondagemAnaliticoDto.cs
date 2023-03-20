namespace SME.SR.Infra
{
    public abstract class RelatorioSondagemAnaliticoDto
    {
        public string Ue {  get; set; }
        public int Ano { get; set; }
        public int TotalDeTurma { get; set; }
        public int TotalDeAlunos { get; set; }
    }
}
