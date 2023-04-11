namespace SME.SR.Infra
{
    public class RespostaMatematicaDto
    {
        public int Acertou {  get; set; }   
        public int Errou { get; set; }
        public int NaoResolveu { get; set; }
        public int SemPreenchimento { get; set; }
    }
}
