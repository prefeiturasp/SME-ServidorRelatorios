namespace SME.SR.Infra
{
    public class ItemRespostaCapacidadeDeLeituraDto : RelatorioSondagemAnaliticoDto
    {
        public int Adequada {  get; set; }
        public int Inadequada { get; set; }
        public int NaoResolveu { get; set; }
        public int SemPreenchimento { get; set; }
    }
}
