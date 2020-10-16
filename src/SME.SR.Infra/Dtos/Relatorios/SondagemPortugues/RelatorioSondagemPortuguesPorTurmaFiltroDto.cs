namespace SME.SR.Infra
{
    public class RelatorioSondagemPortuguesPorTurmaFiltroDto
    {
        public int AnoLetivo { get; set; }
        public int Ano { get; set; }
        public string DreCodigo { get; set; }
        public string UeCodigo { get; set; }
        public long TurmaCodigo { get; set; }
        public ComponenteCurricularSondagemEnum ComponenteCurricularId { get; set; }
        public ProficienciaSondagemEnum ProficienciaId { get; set; }
        public int Semestre { get; set; }
        public string UsuarioRF { get; set; }

    }
}
