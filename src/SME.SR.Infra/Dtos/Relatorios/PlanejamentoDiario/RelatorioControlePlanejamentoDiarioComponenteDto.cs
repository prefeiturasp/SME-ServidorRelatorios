namespace SME.SR.Infra
{
    public class RelatorioControlePlanejamentoDiarioComponenteDto
    {
        public RelatorioControlePlanejamentoDiarioComponenteDto()
        {
            Filtro = new FiltroControlePlanejamentoDiarioDto();
        }
        public FiltroControlePlanejamentoDiarioDto Filtro { get; set; }
    }
}
