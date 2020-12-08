namespace SME.SR.Infra
{
    public class RelatorioLeituraComunicadosDto
    {
        public RelatorioLeituraComunicadosDto()
        {
            Filtro = new FiltroLeituraComunicadosDto(); 
        }

        public FiltroLeituraComunicadosDto Filtro { get; set; }
    }
}
