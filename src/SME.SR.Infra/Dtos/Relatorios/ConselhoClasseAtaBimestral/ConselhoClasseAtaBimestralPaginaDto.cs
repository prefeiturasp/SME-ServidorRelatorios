namespace SME.SR.Infra
{
    public class ConselhoClasseAtaBimestralPaginaDto : ConselhoClasseAtaBimestralDto
    {
        public int NumeroPagina { get; set; }
        public int TotalPaginas { get; set; }
        public bool FinalHorizontal { get; set; }
    }
}
