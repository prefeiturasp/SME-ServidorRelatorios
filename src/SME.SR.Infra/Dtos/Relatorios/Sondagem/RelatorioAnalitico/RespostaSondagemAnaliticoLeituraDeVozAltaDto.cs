namespace SME.SR.Infra
{
    public class RespostaSondagemAnaliticoLeituraDeVozAltaDto : RelatorioSondagemAnaliticoDto
    {
        public int NaoConseguiuOuNaoQuisLer { get; set; }
        public int LeuComMuitaDificuldade { get; set; }
        public int LeuComAlgumaFluência { get; set; }
        public int LeuComFluencia { get; set; }
        public int SemPreenchimento { get; set; }
    }
}
