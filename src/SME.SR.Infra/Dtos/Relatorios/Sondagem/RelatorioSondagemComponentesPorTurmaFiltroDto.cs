namespace SME.SR.Infra
{
    public class RelatorioSondagemComponentesPorTurmaFiltroDto
    {
        public int AnoLetivo { get; set; }
        public string Ano { get; set; }
        public string DreCodigo { get; set; }
        public string UeCodigo { get; set; }
        public long TurmaCodigo { get; set; }
        public ComponenteCurricularSondagemEnum ComponenteCurricularId { get; set; }
        public ProficienciaSondagemEnum ProficienciaId { get; set; }
        public int Semestre { get; set; }
        public int Bimestre { get; set; }
        public string UsuarioRF { get; set; }
    }
}
