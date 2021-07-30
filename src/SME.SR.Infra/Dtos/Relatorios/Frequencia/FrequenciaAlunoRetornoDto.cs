namespace SME.SR.Infra
{
    public class FrequenciaAlunoRetornoDto
    {
        public string TurmaCodigo { get; set; }
        public string AlunoCodigo { get; set; }
        public TipoFrequencia TipoFrequencia { get; set; }
        public int Quantidade { get; set; }
    }
}
