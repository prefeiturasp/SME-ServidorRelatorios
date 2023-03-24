namespace SME.SR.Infra
{
    public class RelatorioPortuguesFiltroDto
    {
        public string GrupoId { get; set; }
        public string ComponenteCurricularId { get; set; }
        public string PeriodoId { get; set; }
        public int AnoLetivo { get; set; }
        public int AnoEscolar { get; set; }
        public string CodigoTurma { get; set; }
        public string CodigoUe { get; set; }
        public string CodigoDre { get; set; }
    }
}