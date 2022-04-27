namespace SME.SR.Infra
{
    public class FrequenciaAlunoMensalConsolidadoDto
    {
        public string DreSigla { get; set; }
        public string UeNome { get; set; }
        public string DescricaoTipoEscola { get; set; }
        public int Mes { get; set; }
        public int ModalidadeCodigo { get; set; }
        public string TurmaNome { get; set; }
        public string CodigoEol { get; set; }
        public decimal Percentual { get; set; }
    }
}
