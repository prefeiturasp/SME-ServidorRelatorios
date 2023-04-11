namespace SME.SR.Infra
{
    public class RelatorioMatematicaFiltroDto
    {
        public ProficienciaSondagemEnum Proficiencia { get; set; }
        public string ComponenteCurricularId { get; set; }
        public int Bimestre { get; set; }
        public int AnoLetivo { get; set; }
        public string CodigoUe { get; set; }
        public string CodigoDre { get; set; }
    }
}