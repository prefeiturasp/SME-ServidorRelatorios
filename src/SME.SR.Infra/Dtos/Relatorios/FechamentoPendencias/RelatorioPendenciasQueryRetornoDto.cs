namespace SME.SR.Infra
{
    public class RelatorioPendenciasQueryRetornoDto
    {
        public long PendenciaId { get; set; }
        public string Titulo { get; set; }
        public string Detalhe { get; set; }
        public int Situacao { get; set; }
        public string DreNome { get; set; }
        public string UeNome { get; set; }
        public int AnoLetivo { get; set; }
        public int ModalidadeCodigo { get; set; }
        public string Semestre { get; set; }
        public string TurmaNome { get; set; }
        public string TurmaCodigo { get; set; }
        public long DisciplinaId { get; set; }
        public int Bimestre { get; set; }
        public string Criador { get; set; }
        public string CriadorRf { get; set; }
        public string Aprovador { get; set; }
        public string AprovadorRf { get; set; }
        public string TipoPendencia { get; set; }
        public bool OutrasPendencias { get; set; }
        public int Tipo { get; set; }
    }
}
